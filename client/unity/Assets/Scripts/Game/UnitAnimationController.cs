using Shared;
using UnityEngine;

namespace Game
{
    public class UnitAnimationController : StateMachineBehaviour
    {
        private bool dieAnimExecuted;

        public static string GetStateName(UnitState state)
        {
            switch (state)
            {
                case UnitState.Attack:
                    return nameof(UnitState.Attack);
                case UnitState.Move:
                    return nameof(UnitState.Move);
                case UnitState.Dead:
                    return "Die_Death";
                case UnitState.Idle:
                    return nameof(UnitState.Idle);
                default:
                    return null;
            }
        }
        
        private void OnEnable()
        {
            dieAnimExecuted = false;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            if (stateInfo.IsName(GetStateName(UnitState.Attack)))
                animator.gameObject.SendMessage(Unit.animEndFuncName, UnitState.Attack);
            else if (stateInfo.IsName(GetStateName(UnitState.Move)))
                animator.gameObject.SendMessage(Unit.animEndFuncName, UnitState.Move);
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            
            if (!dieAnimExecuted && stateInfo.IsName(GetStateName(UnitState.Dead)) && stateInfo.normalizedTime >= 1.0f)
            {
                animator.gameObject.SendMessage(Unit.animEndFuncName, UnitState.Dead);
                dieAnimExecuted = true;
            }
        }

        private void OnValidate()
        {
        }
    }
}
