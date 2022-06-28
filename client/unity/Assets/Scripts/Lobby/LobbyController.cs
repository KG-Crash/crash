using System.Threading.Tasks;
using Module;
using Network;
using UnityEngine;
using Zenject;

public partial class LobbyController : IInitializable
{
    private UI.IntroView introView;
    
    public LobbyController(Dispatcher dispatcher, UI.IntroView introView)
    {
        Handler.Bind(this, dispatcher);
        this.introView = introView;
    }

    public void Initialize()
    {
        UIView.Show(introView).Wait();
    }
}
