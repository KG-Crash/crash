using System;
using Network;
using Protocol.Request;
using System.Threading.Tasks;
using KG;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class IntroPanel : KG.UIView
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private GameObject _connectSpinner;

        public UnityEvent startButtonClick => _startButton.onClick;
        public GameObject connectSpinner => _connectSpinner;
        
#if UNITY_EDITOR
        [Header("Editor")]
        [SerializeField] private KG.ToggleAuto _connectToggleAuto; 
        
        public KG.ToggleAuto connectToggleAuto => _connectToggleAuto;
#endif
    }
}