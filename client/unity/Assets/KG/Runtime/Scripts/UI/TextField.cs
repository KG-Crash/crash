using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KG
{
    [RequireComponent(typeof(InputField))]
    public class TextField : UIAutoComponent<InputField>
    {
        public string text
        {
            get => instance.text;
            set => instance.text = value;
        }

        public InputField.OnChangeEvent onTextChange => instance.onValueChanged;
    }
}
