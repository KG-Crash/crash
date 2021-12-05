using System.Linq;
using FixMath.NET;
using UnityEngine;

namespace Game
{
    public partial class GameController : Unit.Listener
    {
        public void OnDead(Unit unit, Unit from)
        {
            if (from)
                from.owner.exp += (uint)unit.killScore;
            
            unit.animator.SetTrigger("Dead");
        }

        public void OnOwnerChanged(Player before, Player after, Unit unit)
        {
            var beforeString = before != null ? before.ToString(): "null";
            var afterString = after != null ? after.ToString(): "null";
            Debug.Log($"OnOwnerChanged({beforeString}, {afterString}, {unit})");
        }

        public void OnAttack(Unit me, Unit you, Fix64 damage)
        {
            var maxAttackCount = me.animator.GetInteger("MaxAttack");
            var randAttackIndex = Random.Range(0, maxAttackCount);
            me.animator.SetInteger("AttackIndex", randAttackIndex);
            me.animator.SetTrigger("Attack");

            if (you != null && me != null)
                you.AddAttacker(me);
        }

        public void OnDamaged(Unit me, Unit you, Fix64 damage)
        {
            // TODO : Damaged animation
            UnityEngine.Debug.Log($"hp : {me.hp}/{me.maxhp}");
        }

        public void OnHeal(Unit me, Unit you, Fix64 heal)
        {

        }

        public void OnStartMove(Unit unit)
        {
            unit.animator.SetFloat("Forward", 2);
            unit.animator.SetTrigger("Move");
        }

        public void OnEndMove(Unit unit)
        {
            unit.animator.SetFloat("Forward", 0);
        }

        public void OnIdle(Unit unit)
        {
            unit.animator.SetTrigger("Idle");
        }

        public void OnStop(Unit unit)
        {
            unit.animator.SetTrigger("Idle");
        }

        public void OnClear(Unit unit)
        {
            DestroyImmediate(unit.gameObject);
        }

        public void OnFireProjectile(Unit me, Unit you, int projectileOriginID)
		{
            var projectile = _projectilePool.GetProjectile(projectileOriginID, me, you);
            fireHistory.Add(projectile.projectileID, you);
            me.animator.SetTrigger("Attack");
		}
    }
}
