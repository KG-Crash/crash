using System;
using System.Threading.Tasks;
using Module;
using Network;
using UnityEngine;
using UI;

[UIBind(typeof(IntroPanel), true)]
[AutoBindAttribute(flatBuffer: true)]
public partial class IntroState : AppState
{
    [InitializeMethod]
    public void Initialize()
    {
        var view = GetView<IntroPanel>();
        view.startButtonClick.AddListener(OnConnectAsync);
        _ = Client.Instance.Disconnect();
    }
    
    [ClearMethod]
    public void Clear()
    {
        var view = GetView<IntroPanel>();
        view.startButtonClick.RemoveListener(OnConnectAsync);
    }

    private async Task<bool> ConnectAsync()
    {
        var endpoint = "localhost:8000";
        var pair = endpoint.Split(':');
        return await Client.Instance.Connect(pair[0], int.Parse(pair[1]));
    }

    private async void OnConnectAsync()
    {
        while (!await ConnectAsync())
        {
            var retry = await UI.Popup.Boolean("연결 실패, 다시 시도?", "ㄱㄱ", "ㄴㄴ");
            
            if (!retry) 
                break;
        }
    }
}