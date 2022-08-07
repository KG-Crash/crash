using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KG
{
    [RequireComponent(typeof(Button))]
    public class ButtonSingle : UIAutoComponent<Button, Text>
    {
        public UnityEngine.UI.Button.ButtonClickedEvent onClick => instance0.onClick;
        public string text
        {
            get => instance1.text;
            set => instance1.text = value;
        } 
    }
}