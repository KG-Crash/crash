using KG.Util;
using Network;
using Newtonsoft.Json;
using Protocol.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#pragma warning disable 1998

public partial class GameState
{
    private FileLogger _logger;

    public Dictionary<int, User> users { get; private set; } = new Dictionary<int, User>();


    [FlatBufferEvent]
    public async Task<bool> OnActionQueue(ActionQueue response)
    {
        if (_logger != null)
        {
            _logger.Info($"[{response.Sequence}] : {JsonConvert.SerializeObject(response.Actions)}");
        }

        Debug.Log($"receive queue : {response.Turn}, {response.Sequence}, me?={response.Sequence == CrashNetwork.id}");

        actionService.Receive(response);
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnInGameChat(InGameChat response)
    {
        if (_logger != null)
        {
            _logger.Info($"[{response.Sequence}] : {JsonConvert.SerializeObject(response)}");
        }

        Debug.Log(
            $"chat receive queue : {response.Turn}, {response.Sequence},{response.Message} me?={response.Sequence == CrashNetwork.id}");

        actionService.Receive(response);
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnReady(Ready response)
    {
        // 준비가 완료된 유저는 0 based의 Sequence가 발급됨
        // 준비가 안되면 -1로 설정됨
        var allReady = response.Users.All(x => x.Sequence != -1);
        if (allReady == false)
            return true;

        try
        {
            ready = true;
            _logger = new FileLogger($"log/{DateTime.Now}.txt".Replace(":", "_"));

            this.users = response.Users.ToDictionary(x => x.Sequence, x => new User(x.Id, x.Sequence));
            actionService.Setup(this.users.Keys);
            CrashNetwork.id = response.Users.FirstOrDefault(x => x.Id == CrashNetwork.uuid)?.Sequence ?? -1;

            // team : users
            var users = response.Users
                .GroupBy(x => x.Team)
                .ToDictionary(x => x.Key, x => x.ToList());

            // 팀, 플레이어 구성
            foreach (var pair in users)
            {
                var team = teams.Add(pair.Key); // team id
                foreach (var user in pair.Value)
                {
                    team.players.Add(user.Sequence, user.Sequence);
                }
            }

            me = teams.Find(CrashNetwork.id);

            Debug.Log($"OnReady, myname is {CrashNetwork.uuid}");

            foreach (var user in response.Users)
            {
                Debug.Log($"OnReady, user.Id={user.Id}");
            }
            
            ReadyGamePanel(response.Users.Count);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }

        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnResume(Resume response)
    {
        if (!paused)
        {
            Debug.LogError("receive resume but not paused.");
            return true;
        }

        Debug.Log($"user {response.User} resume.");
        paused = false;
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnLeave(LeaveRoom response)
    {
        // TODO: user id로 클라이언트에서 시퀀스를 알아내야함

        var user = this.users.Values.FirstOrDefault(x => x.id == response.User);
        if (user == null)
            return false;

        actionService.Leave(user.sequence);
        
        return true;
    }
}