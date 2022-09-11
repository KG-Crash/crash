using System.Collections;
using System.Collections.Generic;
using KG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AttackTargetView : UIAutoComponent<Image, TextMeshProUGUI>
    {
        public int? attackTarget
        {
            set => instance1.text = value != null? value.ToString(): "X";
        }

        public bool indicator
        {
            set
            {
                var color = value ? Color.gray : Color.white;
                instance0.color = color;  
                instance1.color = color;
            } 
        }
    }
}