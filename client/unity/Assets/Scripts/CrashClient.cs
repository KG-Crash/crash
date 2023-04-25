using System.Threading.Tasks;
using Protocol;

public class CrashClient
{
    public static async Task<T> Request<T>(string api, IProtocol protocol)
        where T : class, IProtocol
    {
        return await Network.Client.Request<T>(CrashResources.LoadServerSettings().lobbyServerAddress, api, protocol);
    }
}