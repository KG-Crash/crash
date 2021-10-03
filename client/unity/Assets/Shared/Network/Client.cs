using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Protocol;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Network
{
    public class Client
    {
        // Events
        public event Action OnConnectFailed;

        // Singleton instance
        private static readonly Lazy<Client> _ist = new Lazy<Client>(() => new Client());
        public static Client Instance => _ist.Value;

        // DotNetty
        private readonly MultithreadEventLoopGroup _multiThreadEventLoopGroup = new MultithreadEventLoopGroup();
        private readonly Bootstrap _bootstrap = new Bootstrap();
        private IChannel _channel = null;

        public bool Connected => _channel?.Active ?? false;

        private Client()
        {
            _bootstrap
            .Group(_multiThreadEventLoopGroup)
            .Channel<TcpSocketChannel>()
            .Option(ChannelOption.TcpNodelay, true)
            .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                channel.Pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.LittleEndian, 1024 * 1024, 0, 4, 0, 4, true));
                channel.Pipeline.AddLast(new LengthFieldPrepender(ByteOrder.LittleEndian, 4, 0, false));
                channel.Pipeline.AddLast(new IdleStateHandler(0, 30, 0));
                channel.Pipeline.AddLast(Handler.Instance);
            }));
        }

        public async Task<bool> Connect(string ip, int port)
        {
            try
            {
                if (Connected)
                    return false;

                _channel = await _bootstrap.ConnectAsync(ip, port);
                return true;
            }
            catch (Exception e)
            {
                //Dispatcher.Instance.Dispatch(() => OnConnectFailed?.Invoke());
                return false;
            }
        }

        public async Task Disconnect()
        {
            if (_channel == null)
                return;

            await _channel.DisconnectAsync();
            _channel = null;
        }

        public async Task Send(IProtocol protocol)
        {
            using (var mstream = new MemoryStream())
            {
                using (var bwriter = new BinaryWriter(mstream))
                {
                    bwriter.Write(protocol.Identity);
                    bwriter.Write(protocol.Serialize());
                    bwriter.Flush();

                    var bytes = mstream.ToArray();
                    await _channel?.WriteAndFlushAsync(Unpooled.Buffer().WriteBytes(bytes));
                }
            }
        }
    }
}
