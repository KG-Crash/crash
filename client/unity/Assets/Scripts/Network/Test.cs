using Network;
using Protocol.Request;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UIView.Show<IntroView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
