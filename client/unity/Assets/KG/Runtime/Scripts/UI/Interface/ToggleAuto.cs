using Michsky.UI.ModernUIPack;
using UnityEngine;

namespace KG
{
    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public class ToggleAuto : UIAutoComponent<UnityEngine.UI.Toggle, UIManagerToggle, CustomToggle>
    {
        public bool on
        {
            get => instance0.isOn;
            set
            {
                instance0.isOn = value;
            }
        } 
        public UnityEngine.UI.Toggle.ToggleEvent onToggle => instance0.onValueChanged;
    }
}