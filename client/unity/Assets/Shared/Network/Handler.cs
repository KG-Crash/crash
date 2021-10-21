using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Protocol;
using Protocol.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

                if (_dispatcher != null)
                    _dispatcher.Dispatch(() => callback.Invoke(allocator.DynamicInvoke(bytes) as IProtocol));
                else
                    callback.Invoke(allocator.DynamicInvoke(bytes) as IProtocol);
            }
            catch (Exception e)
            {
                //UnityEngine.Debug.LogError(e.Message);
            }
        }

        public static void Bind<T>(T t, IDispatchable dispatcher = null) where T : class
        {
            _ist.Value._dispatcher = dispatcher;

            var methods = typeof(T).GetMethods().Where(x =>
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

            foreach (var method in methods)
            {
                try
                {
                    var parameters = method.GetParameters();
                    var protocolType = parameters[0].ParameterType;
                    var instance = Activator.CreateInstance(protocolType) as IProtocol;

                    var allocator = protocolType.GetMethod("Deserialize").CreateDelegate(typeof(Func<,>).MakeGenericType(typeof(byte[]), protocolType));
                    _ist.Value._allocators.Add((Identity)instance.Identity, allocator);

                    var delegateType = Expression.GetDelegateType(parameters.Select(x => x.ParameterType).Concat(new[] { method.ReturnType }).ToArray());
                    var createdDelegate = method.CreateDelegate(delegateType, t);
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
    }
}
