using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameDebugPanel : KG.UIView
    {
        [SerializeField] private Button _btn1;
        [SerializeField] private Button _btn2;
        [SerializeField] private Button _btn3;

        public Button.ButtonClickedEvent onClick1 => _btn1.onClick;
        public Button.ButtonClickedEvent onClick2 => _btn2.onClick;
        public Button.ButtonClickedEvent onClick3 => _btn3.onClick;
    }
}