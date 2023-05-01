using System;
using System.Threading.Tasks;
using UI;
using UnityEngine;

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

#if UNITY_EDITOR
        view.connectToggleAuto.gameObject.SetActive(true);
        view.connectToggleAuto.onToggle.AddListener(OnConnectToggle);
#endif
    }
    
#if UNITY_EDITOR
    private void OnConnectToggle(bool remote)
    {
        CrashResources.LoadServerSettings().connectToRemote = remote;
        GetView<IntroPanel>().connectToggleAuto.on = remote;
    }
#endif
    
    [ClearMethod]
    public void Clear()
    {
        var view = GetView<IntroPanel>();
        view.startButtonClick.RemoveListener(OnConnectAsync);
#if UNITY_EDITOR
        view.connectToggleAuto.onToggle.RemoveListener(OnConnectToggle);
#endif
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
            if (!Now<IntroState>())
                return;
            
            var retry = await UI.Popup.Boolean("Connection Failed, Re?", "GO", "NO");
            if (!retry)
                break;

            view.connectSpinner.SetActive(false);
        }

        await MoveStateAsync<LobbyState>();   
    }
}