using Network;
using Protocol.Request;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyView : UIView
{
    [SerializeField] private Button createGameRoomButton;
    public KG.ScrollView gameRoomList;

    public UnityEvent createGameRoomButtonClick => createGameRoomButton.onClick;  
}
