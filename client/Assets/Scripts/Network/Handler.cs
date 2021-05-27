using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Protocol;
using Protocol.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Network
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FlatBufferEventAttribute : Attribute
    { }

    public class Handler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private static readonly Lazy<Handler> _ist = new Lazy<Handler>(() => new Handler());
        public static Handler Instance => _ist.Value;

        private readonly Dictionary<Identity, Delegate> _allocators = new Dictionary<Identity, Delegate>();
        private readonly Dictionary<Identity, Func<IProtocol, bool>> _bindedEvents = new Dictionary<Identity, Func<IProtocol, bool>>();

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
                callback.Invoke(allocator.DynamicInvoke(bytes) as IProtocol);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }
        }

        public void Bind<T>(T t) where T : class
        {
            var methods = typeof(T).GetMethods().Where(x =>
            {
                if (x.GetCustomAttribute<FlatBufferEventAttribute>() == null)
                    return false;

                if (x.ReturnType != typeof(bool))
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
                    _allocators.Add((Identity)instance.Identity, allocator);

                    var delegateType = Expression.GetDelegateType(parameters.Select(x => x.ParameterType).Concat(new[] { method.ReturnType }).ToArray());
                    var createdDelegate = method.CreateDelegate(delegateType, t);
                    _bindedEvents.Add((Identity)instance.Identity, new Func<IProtocol, bool>((protocol) =>
                    {
                        return (bool)createdDelegate.DynamicInvoke(Convert.ChangeType(protocol, protocolType));
                    }));
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                }
            }
        }
    }
}
