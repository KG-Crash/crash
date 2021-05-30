using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public enum MoveKind
    {
        Step,
        Lerp,
    }

    public class Follower : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private MoveKind _follow;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private MoveKind _switched;

        public Transform target
        {
            get { return _target; }
            set
            {
                _target = value;

                switch (_switched)
                {
                    case MoveKind.Step:
                        transform.position = _target.transform.position;
                        break;
                    case MoveKind.Lerp:
                        break;
                }
            }
        }

        [ContextMenu("Update")]
        private void Update()
        {
            switch (_follow)
            {
                case MoveKind.Step:
                    transform.position = _target.position + _offset;
                    break;
                case MoveKind.Lerp:
                    transform.position = Vector3.Lerp(transform.position, _target.position, 0.1f) + _offset;
                    break;
            }

            transform.LookAt(_target);
        }
    }
}