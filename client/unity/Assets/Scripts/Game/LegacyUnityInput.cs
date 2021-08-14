using System;
using UnityEngine;

namespace Game
{

    public class LegacyUnityInput : MonoBehaviour
    {
        private bool mouseMainBtnPressed = false;
        private bool mouseAltBtnPressed = false;
        
        private void Update()
        {
            var inputBridge = InputBridge._instance;

            var mainFocusPos = Input.mousePosition;
            if (mouseMainBtnPressed)
            {
                inputBridge.OnDragMainBtn(mainFocusPos);
                // 마우스 밖으로 나가면 거시기 처리
            }
            if (Input.GetMouseButtonDown(0))
            {
                mouseMainBtnPressed = true;
                inputBridge.OnPressMainBtn(mainFocusPos);
            }
            if (Input.GetMouseButtonUp(0))
            {
                mouseMainBtnPressed = false;
                inputBridge.OnReleaseMainBtn(mainFocusPos);
            }
            if (mouseAltBtnPressed)
            {
                inputBridge.OnDragAltBtn(mainFocusPos);
                // 마우스 밖으로 나가면 거시기 처리
            }
            if (Input.GetMouseButtonDown(1))
            {
                mouseAltBtnPressed = true;
                inputBridge.OnPressAltBtn(mainFocusPos);
            }
            if (Input.GetMouseButtonUp(1))
            {
                mouseAltBtnPressed = false;
                inputBridge.OnReleaseAltBtn(mainFocusPos);
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

            if (Input.mouseScrollDelta != Vector2.zero)
            {
                inputBridge.OnScrollDelta(Input.mouseScrollDelta.y);
            }
        }
    }
}