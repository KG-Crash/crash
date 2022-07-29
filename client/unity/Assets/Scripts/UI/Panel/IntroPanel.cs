using System;
using Network;
using Protocol.Request;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class IntroPanel : UIView
{
    [SerializeField] private Button _startButton;

    public UnityEvent startButtonClick => _startButton.onClick;
}