using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This script handles canvas states.

public enum CanvasType
{
    pressanybtn,
    MainMenu,
    Chapters,
    Credits,
    Leave,
    Settings,
    Settings_controls,
    Settings_hud,
    Settings_Subtitles,
    Settings_display,
    Settings_audio,
    Settings_language,
    
}

public class StateManager : Singleton<StateManager>

{
    public Animator CanvasAnimator;


    List<StateController> canvasControllerList;
    StateController lastActiveCanvas;


    protected override void Awake()
    {
        base.Awake();
        canvasControllerList = GetComponentsInChildren<StateController>().ToList();
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        StartCoroutine(PlayStateAnimation(CanvasType.pressanybtn));
    }

    public void SwitchCanvas(CanvasType _type)
    {
        if (lastActiveCanvas != null)
        {
            lastActiveCanvas.gameObject.SetActive(false);
        }


        StateController desiredCanvas = canvasControllerList.Find(x => x.canvasType == _type);
        if (desiredCanvas != null)
        {
            desiredCanvas.gameObject.SetActive(true);
            lastActiveCanvas = desiredCanvas;
        }
        else { Debug.LogWarning("The desired canvas was not found!"); }


    }

    public IEnumerator PlayStateAnimation(CanvasType _type)
    {

        CanvasAnimator.Play("out_canvas");
        yield return new WaitForSeconds(0.1f);
        SwitchCanvas(_type);
        CanvasAnimator.Play("in_canvas");

    }




}
