using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using KG;

namespace UI
{
    public class GameRoomPanel : KG.UIView
    {
        [SerializeField] private Button _roomExitButton;
        [SerializeField] private Button _gameStartButton;
    
        [SerializeField] private ScrollView _userNameList;
        [SerializeField] private ChattingView _chattingView;

        public ScrollView userNameList => _userNameList;
        public ChattingView chattingView => _chattingView;

        public UnityEvent roomExitButtonClick => _roomExitButton.onClick;
        public UnityEvent gameStartButtonClick => _gameStartButton.onClick;

        public UnityEvent sendChatButtonClick => _chattingView.sendButton.onClick;
    }
}