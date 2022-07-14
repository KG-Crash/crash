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
        [SerializeField] private MoveKind _switched;

        [SerializeField] private Vector3 _minDistance;
        [SerializeField] private Vector3 _maxDistance;
        [SerializeField] private AnimationCurve _offsetCurveX;
        [SerializeField] private AnimationCurve _offsetCurveY;
        [SerializeField] private AnimationCurve _offsetCurveZ;

        [Range(0, 1)]
        [SerializeField] private float _offsetPosition;
        
        public float offsetPosition
        {
            get => _offsetPosition;
            set => _offsetPosition = Mathf.Clamp01(value);
        }
        
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

        private void OnEnable()
        {
            _offsetPosition = 1;
        }

        [ContextMenu("Update")]
        private void Update()
        {
            Vector3 offset = new Vector3(
                    Mathf.Lerp(_minDistance.x, _maxDistance.x, _offsetCurveX.Evaluate(_offsetPosition)),
                    Mathf.Lerp(_minDistance.y, _maxDistance.y, _offsetCurveY.Evaluate(_offsetPosition)),
                    Mathf.Lerp(_minDistance.z, _maxDistance.z, _offsetCurveZ.Evaluate(_offsetPosition))
                );
            Vector3 targetPosition = _target.position + offset;
            
            switch (_follow)
            {
                case MoveKind.Step:
                    transform.position = targetPosition;
                    break;
                case MoveKind.Lerp:
                    transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
                    break;
            }

            transform.LookAt(_target.position);
        }
    }
}