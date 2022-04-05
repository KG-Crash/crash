using UnityEngine;

namespace Game
{
    public partial class GameController : IInputSubscriber
    {       
        private GameUI _ui;
        private Vector2 _lastPositionSS;
        
        private void InitInput()
        {
            _ui = GetComponent<GameUI>();
            
            InputBridge._instance.Register(this);
        }

        private void ClearInput()
        {
            InputBridge._instance.Unregister(this);
        }

        public void OnPressMainBtn(Vector2 positionSS)
        {
            _lastPositionSS = positionSS;
        }

        public void OnDragMainBtn(Vector2 positionSS)
        {
            var unityObject = UnityResources._instance.Get("objects");
            var focusTransform = unityObject.GetFocus();
            var dragDelta = (float)Shared.Const.Input.DragDelta;
            
            var deltaSS = (positionSS - _lastPositionSS) * dragDelta;
            focusTransform.position += new Vector3(deltaSS.y, 0, -deltaSS.x);

            _lastPositionSS = positionSS;
        }
        public void OnReleaseMainBtn(Vector2 positionSS)
        {            
            _lastPositionSS = positionSS;
        }

        public void OnPressAltBtn(Vector2 positionSS)
        {
        }

        public void OnDragAltBtn(Vector2 positionSS)
        {
            
        }

        public void OnReleaseAltBtn(Vector2 positionSS)
        {
            var positionWS = ScreenPositionToWorldPosition(positionSS);

            // SpawnUnitToPosition(_spawnUnitOriginID, _spawnPlayerID, positionWS, new GameController.TemporalPlaceContext());
        }

        public void OnUpKey()
        {
        }

        public void OnDownKey()
        {
        }

        public void OnLeftKey()
        {
        }

        public void OnRightKey()
        {
        }

        public void OnScrollDelta(float onScrollDelta)
        {
            var unityObject = UnityResources._instance.Get("objects");
            var follower = unityObject.GetCameraFollower();

            follower.offsetPosition -= onScrollDelta * (float)Shared.Const.Input.ScrollDelta;
        }

        public void OnAlphaNum(int num)
        {
            if (!IsNetworkMode)
                FPS = Shared.Const.Time.FPS / num;
        }

        public void OnScroll(float delta)
        {
            
        }
    }
}