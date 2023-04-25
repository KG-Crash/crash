using System;
using System.Threading.Tasks;
using Module;
using Network;
using UnityEngine;
using UI;

[UIBind(typeof(IntroPanel), true)]
[StateBind(flatBuffer: true)]
public partial class IntroState : AppState
{
    [InitializeMethod]
    public void Initialize()
    {
        var view = GetView<IntroPanel>();
        view.startButtonClick.AddListener(OnConnectAsync);
        _ = Client.Instance.Disconnect();

        view.connectSpinner.SetActive(false);

    }
    
    [ClearMethod]
    public void Clear()
    {
        var view = GetView<IntroPanel>();
        view.startButtonClick.RemoveListener(OnConnectAsync);
    }

    private async Task<bool> ConnectAsync()
    {
        try
        {
            var uuid = $"{Guid.NewGuid()}";

            var response = await CrashClient.Request<Protocol.Response.Authentication>(
                "authentication", new Protocol.Request.Authentication {Id = uuid}
            );

            Client.Instance.Token = response.Token;
            Client.Instance.uuid = uuid;
            
            Debug.Log($"response.Token={response.Token}, uuid={uuid}");
            return true;
        }
        catch (Exception)
        {
            return false;
        }

        //var endpoint = "localhost:8000";
        //var pair = endpoint.Split(':');
        //return await Client.Instance.Connect(pair[0], int.Parse(pair[1]));
    }

    private async void OnConnectAsync()
    {
        var view = GetView<IntroPanel>();
        view.connectSpinner.SetActive(true);
        while (!await ConnectAsync())
        {
            var retry = await UI.Popup.Boolean("Connection Failed, Re?", "GO", "NO");
            if (!retry)
                break;

            view.connectSpinner.SetActive(false);
        }

        await MoveStateAsync<LobbyState>();
    }
}