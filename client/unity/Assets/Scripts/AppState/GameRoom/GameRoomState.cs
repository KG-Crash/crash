using GameRoom;
using Protocol.Request;
using UI;

[UIBind(typeof(GameRoomPanel), true)]
[StateBind(flatBuffer: true)]
public partial class GameRoomState : AppState
{
    [InitializeMethod(transition = typeof(GameRoomTransition))]
    public void Initialize(GameRoomTransition transition)
    {
        var view = GetView<GameRoomPanel>();
        view.userNameList.Refresh(new UserListListenerWithButton(Client.Instance.uuid, transition.roomUsers));
        
        view.roomExitButtonClick.AddListener(OnExit);
        view.gameStartButtonClick.AddListener(OnGameStart);
        
        _ = Client.Send(new Chat 
        {
            Message = $"입장 : {Client.Instance.uuid}" 
        });
    }

    [ClearMethod]
    public void Clear()
    {        
        var view = GetView<GameRoomPanel>();
        view.roomExitButtonClick.RemoveListener(OnExit);
        view.gameStartButtonClick.RemoveListener(OnGameStart);
    }
    
    private async void OnGameStart()
    {
        await Client.Send(new GameStart());
    }

    private async void OnExit()
    {
        await Client.Send(new LeaveRoom());
    }
}