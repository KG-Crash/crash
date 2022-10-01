using UnityEngine;

namespace KG
{
    [RequireComponent(typeof(UnityEngine.UI.Toggle))]
    public class Toggle : UIAutoComponent<UnityEngine.UI.Toggle>
    {
        public UnityEngine.UI.Toggle.ToggleEvent onToggle => instance.onValueChanged;
    }
}