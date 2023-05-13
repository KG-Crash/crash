using System.Threading.Tasks;
using Module;
using Network;
using Protocol;

public class CrashClient : BaseClient
{
    protected CrashClient() : base(Dispatcher.Instance) { }

    public async Task<T> Request<T>(string api, IProtocol protocol)
        where T : class, IProtocol
    {
        return await Request<T>(CrashResources.LoadServerSettings().lobbyServerAddress, api, protocol);
    }
}