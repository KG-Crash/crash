using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Protocol;
using Protocol.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FlatBufferEventAttribute : Attribute
    { }


    public interface IDispatchable
    {
        void Dispatch(System.Action action);
    }


    public class Handler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private static readonly Lazy<Handler> _ist = new Lazy<Handler>(() => new Handler());
        public static Handler Instance => _ist.Value;

        private IDispatchable _dispatcher;
        private readonly Dictionary<Identity, Delegate> _allocators = new Dictionary<Identity, Delegate>();
        private readonly Dictionary<Identity, Func<IProtocol, Task<bool>>> _bindedEvents = new Dictionary<Identity, Func<IProtocol, Task<bool>>>();
        private readonly Dictionary<Type, Queue<TaskCompletionSource<IProtocol>>> _tcs = new Dictionary<Type, Queue<TaskCompletionSource<IProtocol>>>();

        public IProtocol Invoke(byte[] bytes)
        {
            try
            {
                using (var stream = new BinaryReader(new MemoryStream(bytes)))
                {
                    var size = stream.ReadInt32();
                    var identity = (Identity)stream.ReadInt32();
                    if (_allocators.TryGetValue(identity, out var allocator) == false)
                        return null;

                    if (_bindedEvents.TryGetValue(identity, out var callback) == false)
                        return null;

                    var pbytes = stream.ReadBytes(size);
                    var protocol = allocator.DynamicInvoke(pbytes) as IProtocol;

                    if (_dispatcher != null)
                        _dispatcher.Dispatch(() => callback.Invoke(protocol));
                    else
                        callback.Invoke(protocol);

                    SetProtocolResult(protocol);

                    return protocol;
                }
            }
            catch (Exception e)
            {
                //UnityEngine.Debug.LogError(e.Message);
                return null;
            }
        }

        private void SetProtocolResult(IProtocol protocol)
        {
            var type = protocol.GetType();

            if (_tcs.TryGetValue(type, out var queue) == false)
            {
                queue = new Queue<TaskCompletionSource<IProtocol>>();
                _tcs.Add(type, queue);
            }

            foreach (var x in queue)
                x.SetResult(protocol);
            queue.Clear();
        }

        public Task<IProtocol> GetProtocolResult<T>(TimeSpan? timeout = null) where T : class, IProtocol
        {
            if (_tcs.TryGetValue(typeof(T), out var queue) == false)
            {
                queue = new Queue<TaskCompletionSource<IProtocol>>();
                _tcs.Add(typeof(T), queue);
            }

            var tcs = new TaskCompletionSource<IProtocol>();
            queue.Enqueue(tcs);

            if (timeout != null)
            {
                var cts = new CancellationTokenSource(timeout.Value);
                cts.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
            }

            return tcs.Task;
        }

        public T Invoke<T>(byte[] bytes) where T : class, IProtocol
        {
            try
            {
                using (var stream = new BinaryReader(new MemoryStream(bytes)))
                {
                    var size = stream.ReadInt32();
                    var identity = (Identity)stream.ReadInt32();
                    var allocator = GetAllocator<T>();

                    var pbytes = stream.ReadBytes(size);
                    var protocol = allocator.DynamicInvoke(pbytes) as T;

                    if (_bindedEvents.TryGetValue(identity, out var callback))
                    {
                        if (_dispatcher != null)
                            _dispatcher.Dispatch(() => callback.Invoke(protocol));
                        else
                            callback.Invoke(protocol);
                    }

                    SetProtocolResult(protocol);
                    return protocol;
                }
            }
            catch (Exception e)
            {
                //UnityEngine.Debug.LogError(e.Message);
                return null;
            }
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            try
            {
                var identity = (Identity)msg.ReadUnsignedIntLE();

                if (_allocators.TryGetValue(identity, out var allocator) == false)
                    return;

                if (_bindedEvents.TryGetValue(identity, out var callback) == false)
                    return;

                var bytes = new byte[msg.ReadableBytes];
                msg.ReadBytes(bytes);

                var protocol = allocator.DynamicInvoke(bytes) as IProtocol;
                if (_dispatcher != null)
                    _dispatcher.Dispatch(() => callback.Invoke(protocol));
                else
                    callback.Invoke(protocol);

                SetProtocolResult(protocol);
            }
            catch (Exception e)
            {
                //UnityEngine.Debug.LogError(e.Message);
            }
        }
        
        private static IEnumerable<MethodInfo> GetMethods(Type type)
        {
            return type.GetMethods().Where(x =>
            {
                if (x.GetCustomAttribute<FlatBufferEventAttribute>() == null)
                    return false;

                if (x.ReturnType != typeof(Task<bool>))
                    return false;

                var parameters = x.GetParameters();
                if (parameters.Length != 1)
                    return false;

                if (parameters[0].ParameterType.GetInterface(nameof(IProtocol)) == null)
                    return false;

                return true;
            });
        }

        private static Delegate GetAllocator<T>() where T : IProtocol
        {
            return GetAllocator(typeof(T));
        }

        private static Delegate GetAllocator(Type type)
        {
            if(type.GetInterface(nameof(IProtocol)) == null)
                throw new Exception();

            return type.GetMethod("Deserialize").CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(byte[]), type));
        }
        
        public static void Bind(object classInstance, IDispatchable dispatcher = null)
        {
            _ist.Value._dispatcher = dispatcher;

            var methods = GetMethods(classInstance.GetType());

            foreach (var method in methods)
            {
                try
                {
                    var parameters = method.GetParameters();
                    var protocolType = parameters[0].ParameterType;
                    var instance = Activator.CreateInstance(protocolType) as IProtocol;

                    _ist.Value._allocators.Add((Identity)instance.Identity, GetAllocator(protocolType));

                    var delegateType = Expression.GetDelegateType(parameters.Select(x => x.ParameterType).Concat(new[] { method.ReturnType }).ToArray());
                    var createdDelegate = method.CreateDelegate(delegateType, classInstance);
                    _ist.Value._bindedEvents.Add((Identity)instance.Identity, new Func<IProtocol, Task<bool>>((protocol) =>
                    {
                        return createdDelegate.DynamicInvoke(Convert.ChangeType(protocol, protocolType)) as Task<bool>;
                    }));
                }
                catch (Exception e)
                {
                    //UnityEngine.Debug.Log(e.Message);
                }
            }
        }

        public static void Unbind(object classInstance)
        {
            var methods = GetMethods(classInstance.GetType());
            
            foreach (var method in methods)
            {
                try
                {
                    var parameters = method.GetParameters();
                    var protocolType = parameters[0].ParameterType;
                    var instance = Activator.CreateInstance(protocolType) as IProtocol;

                    _ist.Value._allocators.Remove((Identity) instance.Identity);
                    _ist.Value._bindedEvents.Remove((Identity) instance.Identity);
                }
                catch (Exception e)
                {
                    //UnityEngine.Debug.Log(e.Message);
                }
            }
        }
    }
}
