using Network;
using Protocol.Request;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    [UI("IntroView")]
    public class IntroView : UIView
    {
        [SerializeField] public Button connectButton;

        public Button.ButtonClickedEvent onClickConnect => connectButton.onClick; 
    }
}