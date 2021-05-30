using System;
using UnityEngine;

namespace Game
{

    public class LegacyUnityInput : MonoBehaviour
    {
        private bool mousePressed = false;
        
        private void Update()
        {
            var inputBridge = InputBridge._instance;

            var mainFocusPos = Input.mousePosition;
            if (mousePressed)
            {
                Debug.Log("else");
                inputBridge.OnDragMainBtn(mainFocusPos);
            }
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Input.GetMouseButtonUp(0)");
                mousePressed = false;
                inputBridge.OnReleaseMainBtn(mainFocusPos);
            }
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Input.GetMouseButtonDown(0)");
                mousePressed = true;
                inputBridge.OnPressMainBtn(mainFocusPos);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                inputBridge.OnUpKey();
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                inputBridge.OnDownKey();
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                inputBridge.OnLeftKey();
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                inputBridge.OnRightKey();
            }

        }
    }
}