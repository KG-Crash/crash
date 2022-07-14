using FixMath.NET;
using Game.Service;
using Module;
using Network;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using Game;

public partial class GameState : ActionService.Listener
{
    private void OnUpdate()
    {
        OnUpdateAlwaysDebug();

        if (!ready || paused)
        {
            return;
        }

        waitPacket = LockStep.Turn.In > LockStep.Turn.Out + 2;

        if (actionService.Update())
            LockStep.Turn.Out++;

        if (waitPacket)
        {
            return;   
        }
        
        EnqueueHeartBeat();
        _me.upgrade.Update(OutputFrameChunk);
        OnUpdateFrameDebug(OutputFrameChunk);
    }

    private void OnLateUpdate()
    {
        if (!ready || paused || waitPacket)
        {
            return;
        }
        
        Debug.Log($"LockStep.Turn.In({LockStep.Turn.In}) > LockStep.Turn.Out({LockStep.Turn.Out}) + 2");

        if (++LockStep.Frame.In >= Shared.Const.Time.FramePerTurn)
        {
            if (IsNetworkMode)
            {
                actionService.Flush();
                Debug.Log($"send queue : {LockStep.Turn.In}");
            }

            LockStep.Turn.In++;
            LockStep.Frame.In = 0;
        }

        waitPacket = LockStep.Turn.In > LockStep.Turn.Out + 2;
    }

    public void OnAction(int userId, Protocol.Response.Action action)
    {
        ActionHandler.Execute<GameState>(userId, action);
    }

    public void OnChat(int userId, Protocol.Response.InGameChat chat)
    {
        if (uuidTable.TryGetValue(userId, out var name) == false)
            return;

        chatService.RecvMessage(chat.Message, $"{name}");
    }
}