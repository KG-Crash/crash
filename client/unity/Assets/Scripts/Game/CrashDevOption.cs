using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    [CreateAssetMenu(fileName = "CrashDevOption", menuName = "Crash/DevOption", order = 0)]
    public class CrashDevOption : ScriptableObject
    {
        private static CrashDevOption _instance { get; set; }
        public static float cameraMoveDelta {
            get => CrashDevOption._instance._cameraMoveDelta;
        }

        [SerializeField]
        public float _cameraMoveDelta = 10.0f;

        private void OnEnable()
        {
            Assert.IsNull(_instance);
            _instance = this;
        }
    }
}