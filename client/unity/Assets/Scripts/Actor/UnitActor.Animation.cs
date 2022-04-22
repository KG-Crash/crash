using System;
using System.Collections.Generic;
using Shared;
using UnityEngine;

namespace Game
{   
    public class StateMachineCallbackAttribute : Attribute
    {
        public string _name;

        public StateMachineCallbackAttribute(string name)
        {
            _name = name;
        }
    }

    public partial class UnitActor
    {
        [Header("Animation")]
        [SerializeField] private int _maxAttackAnimCount;

        public static string AnimEndFuncName => nameof(OnAnimEnd);
        
        public void InitAnimation()
        {
            animator.SetInteger("MaxAttack", _maxAttackAnimCount);
        }
        
        public int maxAttackAnimCount
        {
            get => _maxAttackAnimCount;
            set => _maxAttackAnimCount = value;
        }

        [StateMachineCallback(nameof(OnAnimEnd))]
        private void OnAnimEnd(UnitState state)
        {
            switch (state)
            {
                case UnitState.Attack:
                    break;
                case UnitState.Move:
                    break;
                case UnitState.Dead:
                    StartCoroutine(OnDisappearAnim());
                    break;
                case UnitState.Idle:
                    break;
            }
        }
        
        private IEnumerator<Unit> OnDisappearAnim()
        {
            var duration = 1.0f;
            var startTime = Time.time;

            SetFadeMaterialAndAlpha(1.0f);

            while (startTime + duration >= Time.time)
            {
                var alpha = 1.0f - (Time.time - startTime) / duration;
                SetFadeAlpha(alpha);

                yield return null;
            }

            SetFadeAlpha(0.0f);
            
            _listener?.OnClear(this);
        }
    }
}