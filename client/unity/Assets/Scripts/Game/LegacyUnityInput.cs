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
                Debug.Log("Input.GetMouseButtonUp(0)");
                
                // 마우스 밖으로 나가면 거시기 처리
            }
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Input.GetMouseButtonDown(0)");
                mousePressed = true;
                inputBridge.OnPressMainBtn(mainFocusPos);
            }
            if (Input.GetMouseButtonUp(0))
            {
                mousePressed = false;
                inputBridge.OnReleaseMainBtn(mainFocusPos);
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                inputBridge.OnUpKey();
            }

            if (Input.GetKey(KeyCode.DownArrow)|| Input.GetKey(KeyCode.S))
            {
                inputBridge.OnDownKey();
            }

            if (Input.GetKey(KeyCode.LeftArrow)|| Input.GetKey(KeyCode.A))
            {
                inputBridge.OnLeftKey();
            }

            if (Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.D))
            {
                inputBridge.OnRightKey();
            }

        }
    }
}