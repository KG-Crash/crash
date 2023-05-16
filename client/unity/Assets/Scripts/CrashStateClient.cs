using System;
using System.Threading.Tasks;
using Module;
using Network;
using Protocol;
using UnityEngine;

public struct LogScope : IDisposable
{
    private string prefix;

    public LogScope(string prefix)
    {
        this.prefix = prefix; 
        if (CrashResources.LoadServerSettings().logSendRequest) 
            Debug.Log($"{prefix}, Start");
    }

    public void Dispose()
    {
        if (CrashResources.LoadServerSettings().logSendRequest)
            Debug.Log($"{prefix}, End");  
    } 
}

public class CrashStateClient : BaseClient
{
    public override string Token
    {
        get => CrashNetwork.token;
        set => CrashNetwork.token = value;
    }

    protected CrashStateClient() : base(Dispatcher.Instance) { }

    public async Task<T> Request<T>(string api, IProtocol protocol)
        where T : class, IProtocol
    {
        using (new LogScope($"Request<{typeof(T)}>, http, {api}"))
            return await base.Request<T>(CrashResources.LoadServerSettings().lobbyServerAddress, api, protocol);
    }

    public new async Task<T> Request<T>(IProtocol protocol, TimeSpan? timeout = null) where T : class, IProtocol
    {
        using (new LogScope($"Request<{typeof(T)}>, socket, {timeout}"))
            return await base.Request<T>(protocol, timeout);
    }

    public new async Task Send(IProtocol protocol)
    {
        using (new LogScope($"Send, {protocol.GetType()}"))
            await base.Send(protocol);
    }
}