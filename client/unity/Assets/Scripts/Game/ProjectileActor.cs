using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared;
using Shared.Table;
using System;
using FixMath.NET;

namespace Game
{
    public class ProjectileActor : MonoBehaviour, IActor
    {
        public interface Listener
        {
        }

        public Listener listener { get; private set; }

        public FixVector3 position
        {
            set => transform.position = value;
        }

        public void LookAt(FixVector3 worldPosition)
        {
            transform.LookAt(worldPosition);
        }

        public Transform parent => transform.parent;
        
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }

        public void Init(Listener listener)
        {
            this.listener = listener;
        }
    }
}