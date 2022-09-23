using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

namespace KG
{
    [RequireComponent(typeof(Button))]
    public class ButtonSingleTMP : UIAutoComponent<Button, ButtonManagerWithIcon>
    {
        public UnityEngine.UI.Button.ButtonClickedEvent onClick => instance0.onClick;
        public string text
        {
            get => instance1.buttonText;

            set
            {
                instance1.buttonText = value;
                instance1.UpdateUI();
            }
        } 
        public Color color
        {
            get => instance1.startColor;
            set => instance1.startColor=value;
        }
    }
}