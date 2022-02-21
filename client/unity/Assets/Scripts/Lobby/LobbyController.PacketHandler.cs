using Network;
using Protocol.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomListListener : KG.ScrollView.IListener<string>
{
    public RoomList roomList { get; private set; }

    public RoomListListener(RoomList response)
    {
        roomList = response;
    }

    public void OnCreated(string data, UIBehaviour item)
    {
        item.GetComponent<GameRoomButton>().RoomId = data;

        var button = item.GetComponent<Button>();
        button.GetComponentInChildren<Text>().text = data;
    }

    public void OnDestroyed(UIBehaviour item)
    { }

    public IEnumerator<string> OnRefresh()
    {
        return roomList.Rooms.Select(x => x.Id).GetEnumerator();
    }
}

public class UserListListener : KG.ScrollView.IListener<string>
{
    private string _mine;
    private readonly List<string> _idList = new List<string>();

    public UserListListener(string mine, IEnumerable<string> idList)
    {
        _mine = mine;
        _idList = idList.ToList();
    }

    public void OnCreated(string data, UIBehaviour item)
    {
        var text = item.GetComponent<Text>();
        text.text = data;

        var mine = (data == _mine);
        if (mine)
            text.color = UnityEngine.Color.blue;
    }

    public void OnDestroyed(UIBehaviour item)
    {

    }

    public IEnumerator<string> OnRefresh()
    {
        return _idList.GetEnumerator();
    }
}

public class ChatLogListener : KG.ScrollView.IListener<string>
{
    private readonly Chat _response;

    public ChatLogListener(Chat response)
    {
        _response = response;
    }
    public void OnCreated(string data, UIBehaviour item)
    {
        var text = item.GetComponent<Text>();
        text.text = data;

        var mine = (data == _response.User);
        if (mine)
            text.color = UnityEngine.Color.blue;
    }

    public void OnDestroyed(UIBehaviour item)
    {

    }

    public IEnumerator<string> OnRefresh()
    {
        return null;
    }
}

public partial class LobbyController : MonoBehaviour
{
    [FlatBufferEvent]
    public async Task<bool> OnLogin(Login response)
    {
        Client.Instance.id = response.Id;
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnCreateRoom(CreateRoom response)
    {
        var view = await UIView.Show<GameRoomView>();
        view.userNameList.Refresh(new UserListListener(Client.Instance.id, new string[] { Client.Instance.id }));
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnEnterRoom(EnterRoom response)
    {
        var isMine = (response.User == Client.Instance.id);

        var view = isMine ? await UIView.Show<GameRoomView>() : UIView.Get<GameRoomView>();
        view.userNameList.Refresh(new UserListListener(Client.Instance.id, response.Users.Select(x => x.Id)));
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnRoomList(RoomList response)
    {
        var uiView = UIView.Get<LobbyView>();
        var listener = new RoomListListener(response);
        uiView.gameRoomList.Refresh(listener);
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnChat(Chat response)
    {
        UnityEngine.Debug.Log(response.Message);
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnWhisper(Whisper response)
    {
        if (Client.Instance.id == response.From)
        {
            UnityEngine.Debug.Log($"{response.To} << {response.Message}");
        }
        else
        {
            UnityEngine.Debug.Log($"{response.From} >> {response.Message}");
        }
        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnLeaveRoom(LeaveRoom response)
    {
        var isMine = (response.User == Client.Instance.id);

        if (isMine)
        {
            await UIView.Get<LobbyView>()?.Refresh();
            await UIView.Close();
        }
        else
        {
            UnityEngine.Debug.Log($"{response.User} 가 나감.");
        }

        return true;
    }

    [FlatBufferEvent]
    public async Task<bool> OnGameStart(GameStart response)
    {
        if (response.Error != 0)
        {
            UnityEngine.Debug.LogError("게임을 시작할 수 없음");
            return true;
        }

        //Client.Instance.seed = response.Seed;
        SceneManager.LoadSceneAsync("OnlineScene");
        return true;
    }
}