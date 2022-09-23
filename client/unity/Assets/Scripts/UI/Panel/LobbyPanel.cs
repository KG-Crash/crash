using Network;
using Protocol.Request;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class LobbyPanel : KG.UIView
    {
        [SerializeField] private Button _createGameRoomButton;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private KG.ScrollView _gameRoomList;

        public KG.ScrollView gameRoomList => _gameRoomList;
        public UnityEvent createGameRoomButtonClick => _createGameRoomButton.onClick;
        public UnityEvent refreshButton => _refreshButton.onClick;
    }
}