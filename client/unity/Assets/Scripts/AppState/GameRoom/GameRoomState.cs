using GameRoom;
using Protocol.Request;
using UI;

[UIBind(typeof(GameRoomPanel), true)]
[StateBind(flatBuffer: true)]
public partial class GameRoomState : AppState
{
    public GameRoomState() : base() {}
    
    [InitializeMethod(transition = typeof(GameRoomTransition))]
    public void Initialize(GameRoomTransition transition)
    {
        var view = GetView<GameRoomPanel>();
        view.userNameList.Refresh(new UserListListenerWithButton(uuid, transition.roomUsers));
        
        view.roomExitButtonClick.AddListener(OnExit);
        view.gameStartButtonClick.AddListener(OnGameStart);
        
        _ = Send(new Chat 
        {
            Message = $"입장 : {uuid}" 
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
        await Send(new GameStart());
    }

    private async void OnExit()
    {
        await Send(new LeaveRoom());
    }
}