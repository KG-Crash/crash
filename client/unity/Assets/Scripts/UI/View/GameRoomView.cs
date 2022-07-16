using Network;
using Protocol.Request;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameRoomView : UIView
{
    [SerializeField] private Button roomExitButton;
    [SerializeField] private Button gameStartButton;
    
    public KG.ScrollView userNameList;
    public KG.ScrollView chatLogList;

    public UnityEvent roomExitButtonClick => roomExitButton.onClick;
    public UnityEvent gameStartButtonClick => gameStartButton.onClick;
}
