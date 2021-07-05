using Shared;
using UnityEngine;

namespace Game
{
    public class UnitAnimationController : StateMachineBehaviour
    {
        private bool dieAnimExecuted;
        
        private void Awake()
        {
        }

        private void Reset()
        {
        }

        private void OnEnable()
        {
            dieAnimExecuted = false;
        }

        private void OnDisable()
        {
        }

        private void OnDestroy()
        {
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            
            if (stateInfo.IsName("Attack"))
                animator.gameObject.SendMessageUpwards("OnAnimEnd", UnitState.Attack);
            else if (stateInfo.IsName("Move"))
                animator.gameObject.SendMessageUpwards("OnAnimEnd", UnitState.Move);
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateIK(animator, stateInfo, layerIndex);
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateMove(animator, stateInfo, layerIndex);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            
            if (!dieAnimExecuted && stateInfo.IsName("Die") && stateInfo.normalizedTime >= 1.0f)
            {
                animator.gameObject.SendMessageUpwards("OnAnimEnd", UnitState.Dead);
                dieAnimExecuted = true;
            }
        }

        private void OnValidate()
        {
        }
    }
}
