using System;
using System.Threading.Tasks;
using Module;
using Network;
using Protocol.Response;
using UnityEngine;

public partial class IntroState
{
    [FlatBufferEvent]
    public async Task<bool> OnLogin(Login response)
    {
        Client.Instance.uuid = response.Id;
        await MoveStateAsync<LobbyState>();
        return true;
    }
}