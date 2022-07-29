using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameRoomPanel : UIView
{
    [SerializeField] private Button _roomExitButton;
    [SerializeField] private Button _gameStartButton;
    
    [SerializeField] private KG.ScrollView _userNameList;
    [SerializeField] private KG.ScrollView _chatLogList;

    public KG.ScrollView userNameList => _userNameList;
    public KG.ScrollView chatLogList => _chatLogList;

    public UnityEvent roomExitButtonClick => _roomExitButton.onClick;
    public UnityEvent gameStartButtonClick => _gameStartButton.onClick;
}
