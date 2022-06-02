using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StateSwitcher : MonoBehaviour
{
    public CanvasType desiredCanvasType;

    StateManager stateManager;
    Button menuButton;

    private void Start()
    {
        menuButton = GetComponent<Button>();
        menuButton.onClick.AddListener(OnButtonClicked);
        stateManager = StateManager.GetInstance();
    }

    void OnButtonClicked()
    {
       StartCoroutine(stateManager.PlayStateAnimation(desiredCanvasType));
    }
}