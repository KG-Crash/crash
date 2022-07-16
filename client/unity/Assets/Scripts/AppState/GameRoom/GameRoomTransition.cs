using System;
using Module;
using Network;
using UnityEngine;
using Protocol.Request;

public class GameRoomTransition : StateTransition
{
    public bool isMine { get; private set; }
    public string[] roomUsers { get; private set; }

    public GameRoomTransition(bool isMine, string[] roomUsers)
    {
        this.isMine = isMine;
        this.roomUsers = roomUsers;
    }
}