using UnityEngine;

namespace KG
{
    [RequireComponent(typeof(MultiClickButton))]
    public class MultiButtonSingle : UIAutoComponent<MultiClickButton, TextMeshProLabel>
    {
        public MultiClickButton.ButtonClickedEvent onClick => instance0.onClick;
        public string text
        {
            get => instance1.text;
            set => instance1.text = value;
        } 
    }
}