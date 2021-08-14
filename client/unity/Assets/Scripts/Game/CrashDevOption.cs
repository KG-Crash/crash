using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    [CreateAssetMenu(fileName = "CrashDevOption", menuName = "Crash/DevOption", order = 0)]
    public class CrashDevOption : ScriptableObject
    {
        public static float cameraMoveDelta 
        {
            get => CrashDevOption._instance._cameraMoveDelta;
        }

        public static float dragDelta
        {
            get => CrashDevOption._instance._dragDelta;
        }
        
        private static CrashDevOption _instance { get; set; }

        [SerializeField]
        public float _cameraMoveDelta = 10.0f;

        [SerializeField]
        public float _dragDelta = 0.1f;

        private void OnEnable()
        {
            Assert.IsNull(_instance);
            _instance = this;
        }
    }
}