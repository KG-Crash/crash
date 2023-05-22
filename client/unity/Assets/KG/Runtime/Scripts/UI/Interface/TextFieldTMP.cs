using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.ModernUIPack;

namespace KG
{
    [RequireComponent(typeof(TMP_InputField))]
    public class TextFieldTMP : UIAutoComponent<TMP_InputField>
    {
        public string text
        {
            get => instance.text;
            set => instance.text = value;
        }

        public TMP_InputField.OnChangeEvent onTextChange => instance.onValueChanged;
    }
}
