using GameRoom;
using Protocol.Response;
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
        view.roomExitButtonClick.AddListener(OnExit);
        view.gameStartButtonClick.AddListener(OnGameStart);
        view.sendChatButtonClick.AddListener(OnChatSend);

        Connect(transition);
    }

    public async void Connect(GameRoomTransition transition)
    {
        if (await Connect(transition.Host, (int) transition.Port) == false)
        {
            // TODO: 게임서버에 연결못했을 때 에러처리
            return;
        }

        var _ = await Request<Protocol.Response.Login>(new Protocol.Request.Login
        {
            Id = CrashNetwork.uuid
        });

        if (transition.Enter)
        {
            await Send(new Protocol.Request.EnterRoom {Id = transition.RoomId});
        }
        else
        {
            await Request<Protocol.Response.CreateRoom>(new Protocol.Request.CreateRoom
            {
                Id = transition.RoomId,
                Title = "my game room title",
                Teams = new System.Collections.Generic.List<int>
                {
                    2, 2 // 2 vs 2
                }
            });
        }
    }

    [ClearMethod]
    public void Clear()
    {        
        var view = GetView<GameRoomPanel>();
        view.roomExitButtonClick.RemoveListener(OnExit);
        view.gameStartButtonClick.RemoveListener(OnGameStart);
        view.sendChatButtonClick.RemoveListener(OnChatSend);
    }
    
    private async void OnGameStart()
    {
        await Send(new GameStart());
    }

    private async void OnExit()
    {
        await Send(new LeaveRoom());
    }

    private async void OnChatSend()
	{
        var msg = GetView<GameRoomPanel>().chattingView.inputFieldText;
        await Send(new Protocol.Request.Chat { Message = msg }); 
	}
}