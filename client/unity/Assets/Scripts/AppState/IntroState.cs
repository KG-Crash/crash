using System;
using System.Threading.Tasks;
using Module;
using Network;
using UnityEngine;

[UIBind(typeof(IntroView), true)]
[AutoBindAttribute(flatBuffer: true)]
public partial class IntroState : AppState
{
    [InitializeMethod]
    public void Initialize()
    {
        var view = GetView<IntroView>();
        view.startButtonClick.AddListener(OnConnectAsync);
        _ = Client.Instance.Disconnect();
    }
    
    [ClearMethod]
    public void Clear()
    {
        var view = GetView<IntroView>();
        view.startButtonClick.RemoveListener(OnConnectAsync);
        _ = UIView.Close();
    }

    private async void OnConnectAsync()
    {
        var endpoint = "localhost:8000";
        var pair = endpoint.Split(':');
        var connection = await Client.Instance.Connect(pair[0], int.Parse(pair[1]));
        if (!connection)
        {
            // 실패 어찌?
        }
    }
}