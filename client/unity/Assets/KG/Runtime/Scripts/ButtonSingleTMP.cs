using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
namespace KG
{
    [RequireComponent(typeof(Button))]
    public class ButtonSingleTMP : UIAutoComponent<Button, TextMeshProUGUI>
    {
        public UnityEngine.UI.Button.ButtonClickedEvent onClick => instance0.onClick;
        public string text
        {
            get => instance1.text;
            
            set => instance1.text = value;
        } 
    }
}