using UnityEngine;
using UnityEngine.UI;

namespace KG
{
    [RequireComponent(typeof(Text))]
    public class TextLabel : UIAutoComponent<Text>
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