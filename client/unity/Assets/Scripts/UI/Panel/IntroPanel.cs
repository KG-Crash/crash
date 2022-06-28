using System;
using System.Threading.Tasks;
using Network;
using UniRx;
using UnityEngine.Events;

namespace UI
{
    public class IntroPanel : IDisposable
    {
        private IntroView introView;
        private UnityAction clickAction;
    
        public IntroPanel(IntroView introView, LobbyView lobbyView, Client client)
        {
            this.introView = introView;
            introView.onClickConnect.AsObservable()
                .Subscribe(_ => OnConnect(client, lobbyView, "localhost:8000"));
            introView.onLoad += OnLoad; 
        }

        public void Dispose()
        {
            introView.onLoad -= OnLoad;
        }
    
        async void OnConnect(Client client, LobbyView lobbyView, string endpoint)
        {
            var pair = endpoint.Split(':');
            if (await client.Connect(pair[0], int.Parse(pair[1])))
            {
                await UIView.Show<LobbyView>(lobbyView, hideBackView: true);
            }
        }
    
        async Task OnLoad()
        {
            await Client.Instance.Disconnect();
        }
    }    
}