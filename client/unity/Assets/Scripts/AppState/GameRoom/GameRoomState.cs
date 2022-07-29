using System;
using Module;
using Network;
using UnityEngine;
using Protocol.Request;
using GameRoom;

[UIBind(typeof(GameRoomPanel), true)]
[AutoBindAttribute(flatBuffer: true)]
public partial class GameRoomState : AppState
{
    [InitializeMethod]
    public void Initialize(GameRoomTransition transition)
    {
        var view = GetView<GameRoomPanel>();
        view.userNameList.Refresh(new UserListListener(Client.Instance.uuid, transition.roomUsers));
        
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