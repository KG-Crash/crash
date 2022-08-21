using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KG
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProLabel : UIAutoComponent<TextMeshProUGUI>
    {
        public string text
        {
            get => instance.text;
            set => instance.text = value;
        }

        public Color textColor
        {
            get => instance.color;
            set => instance.color = value;
        }
    }
}