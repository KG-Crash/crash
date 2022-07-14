using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UnityObjects : MonoBehaviour
    {
        [SerializeField] private List<Camera> _cameras;
        [SerializeField] private Transform _focus;
        [SerializeField] private List<Light> _lights;
        
        private void Awake()
        {
            UnityResources._instance.Register(this);
        }

        private void OnDestroy()
        {
            UnityResources._instance.Unregister(this);
        }

        public Camera GetCamera()
        {
            return _cameras[0];
        }

        public Follower GetCameraFollower()
        {
            return _cameras[0].GetComponent<Follower>();   
        }
        

        public Transform GetFocus()
        {
            return _focus;
        }
    }
}