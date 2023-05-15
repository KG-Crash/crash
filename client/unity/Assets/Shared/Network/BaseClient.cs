using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Protocol;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Network
{
    public class BaseClient
    {
        // Events
        public event Action OnConnectFailed;

        // DotNetty
        private Handler _handler;
        private readonly MultithreadEventLoopGroup _multiThreadEventLoopGroup = new MultithreadEventLoopGroup();
        private readonly Bootstrap _bootstrap = new Bootstrap();
        private IChannel _channel = null;

        public bool Connected => _channel?.Active ?? false;
        public virtual string Token { get; set; } = null;

        protected BaseClient(IDispatchable dispatcher = null)
        {
            _handler = new Handler(this, dispatcher);

            _bootstrap
            .Group(_multiThreadEventLoopGroup)
            .Channel<TcpSocketChannel>()
            .Option(ChannelOption.TcpNodelay, true)
            .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                channel.Pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.LittleEndian, 1024 * 1024, 0, 4, 0, 4, true));
                channel.Pipeline.AddLast(new LengthFieldPrepender(ByteOrder.LittleEndian, 4, 0, false));
                channel.Pipeline.AddLast(new IdleStateHandler(0, 30, 0));
                channel.Pipeline.AddLast(_handler);
            }));
        }

        public void CopyTo(BaseClient other)
        {
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

        public async Task<T> Request<T>(IProtocol protocol, TimeSpan? timeout = null) where T : class, IProtocol
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

            return await _handler.GetProtocolResult<T>(timeout) as T;
        }

        public async Task<T> Request<T>(string host, string api, IProtocol protocol)
            where T : class, IProtocol
        {
            var url = $"{host}/{api}";
            using (var mstream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(mstream))
                {
                    var serialized = protocol.Serialize();
                    writer.Write(serialized.Length + sizeof(uint));
                    writer.Write(protocol.Identity);
                    writer.Write(serialized);
                    writer.Flush();

                    var bytes = mstream.ToArray();

                    var request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "POST";
                    request.ContentType = "application/octet-stream";
                    request.ContentLength = bytes.Length;
                    if(string.IsNullOrEmpty(Token) == false)
                        request.Headers.Add("Authorization", $"Bearer {Token}");

                    using (var stream = await request.GetRequestStreamAsync())
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }

                    using (var resp = await request .GetResponseAsync() as HttpWebResponse)
                    {
                        var status = resp.StatusCode;
                        if (status != HttpStatusCode.OK)
                            throw new Exception($"http request error : {status}");

                        var buffer = new byte[resp.ContentLength];
                        var read = 0;
                        using (var sr = resp.GetResponseStream())
                        {
                            while (true)
                            {
                                var remain = (int)(resp.ContentLength - read);
                                if (remain == 0)
                                    break;

                                read += sr.Read(buffer, read, remain);
                            }
                        }

                        return _handler.Invoke<T>(buffer);
                    }
                }
            }
        }
    }
}
