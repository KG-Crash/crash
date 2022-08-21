using System;
using Network;
using Protocol.Request;
using System.Threading.Tasks;
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
    }
}