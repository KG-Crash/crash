using Module;
using Network;
using UnityEngine;

public partial class LobbyController : MonoBehaviour
{
    private void Awake()
    {
        Handler.Bind(this, Dispatcher.Instance);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
