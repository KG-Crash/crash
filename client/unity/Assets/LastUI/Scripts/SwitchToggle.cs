using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//This script handles Toggle.
//You need to import DOTween package in order to use.

public class SwitchToggle : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] Text indicator;
    [Header("Handle")]
    [SerializeField] RectTransform Handle;
    [Header("Color Sttings")]
    [SerializeField] Color backgroundActiveColor;
    [SerializeField] Color handleActiveColor;


    Image backgroundImage, handleImage;

    Color backgroundDefaultColor, handleDefaultColor;

    Toggle toggle;

    Vector2 handlePosition;

    void Awake()
    {
        toggle = GetComponent<Toggle>();

        handlePosition = Handle.anchoredPosition;

        backgroundImage = Handle.parent.GetComponent<Image>();
        handleImage = Handle.GetComponent<Image>();

        backgroundDefaultColor = backgroundImage.color;
        handleDefaultColor = handleImage.color;

        toggle.onValueChanged.AddListener(OnSwitch);

        if (toggle.isOn)
            OnSwitch(true);
    }

    void OnSwitch(bool on)
    {
        
        indicator.text = on ? (indicator.text = "ON") : indicator.text = "OFF";
        indicator.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, 1f);

        //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition ; // no anim
        Handle.DOAnchorPos(on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack);

        //backgroundImage.color = on ? backgroundActiveColor : backgroundDefaultColor ; // no anim
        backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, 1f);

        //handleImage.color = on ? handleActiveColor : handleDefaultColor ; // no anim
        handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, 1f);
        
    }


    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}