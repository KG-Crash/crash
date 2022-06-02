using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoItems : MonoBehaviour
{

    public HorizontalSelector colorSelector;
    
    public Slider slider;

    public Image box;

    private void Start()
    {
        //This is for Slider to control image opacity.
        box.color = new Color(box.color.r, box.color.g, box.color.b, slider.value);
    }
    private void Update()
    {

        //This is for Horizontal Selector to control color of the Image.
        switch (colorSelector.value)
        {
            case "RED":
                box.color = new Color(1f, 0.258f, 0.258f, slider.value);
                break;
            case "GREEN":
                box.color = new Color(0.278f, 1, 0.380f, slider.value);
                break;
            case "BLUE":
                box.color = new Color(0.278f, 0.580f, 1, slider.value);
                break;
            case "MAGENTA":
                box.color = new Color(0.964f, 0.278f, 1, slider.value);
                break;
            case "BLACK":
                box.color = new Color(0, 0, 0, slider.value);
                break;
        }
    }

    //This is for toggle to active or deactive Image.
    public void ActiveDeactiveObject()
    {
        if (box.gameObject.activeSelf == true)
        {
            box.gameObject.SetActive(false);
        }
        else
        {
            box.gameObject.SetActive(true);
        }
    }
}
