using FixMath.NET;
using UnityEngine;

namespace Game
{
    public partial class GameController : Unit.Listener, UnitActor.Listener
    {
        public void OnDead(Unit unit, Unit from)
        {
            if (from != null)
                from.owner.exp += (uint)unit.killScore;

            if (unitActorMaps.TryGetValue(unit, out var actor) == false)
                return;

            actor.animator.SetTrigger("Dead");
            unit.Destroy();
        }

        public void OnRemove(Unit unit)
        {
            unitActorMaps.Remove(unit);
        }

        public void OnOwnerChanged(Player before, Player after, Unit unit)
        {
            var beforeString = before != null ? before.ToString(): "null";
            var afterString = after != null ? after.ToString(): "null";
            // Debug.Log($"OnOwnerChanged({beforeString}, {afterString}, {unit})");
        }

        public void OnAttack(Unit me, Unit you, Fix64 damage)
        {
            if (unitActorMaps.TryGetValue(me, out var myActor) == false)
                return;

            var maxAttackCount = myActor.animator.GetInteger("MaxAttack");
            var randAttackIndex = Random.Range(0, maxAttackCount);
            myActor.animator.SetInteger("AttackIndex", randAttackIndex);
            myActor.animator.SetTrigger("Attack");

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
            if (unitActorMaps.TryGetValue(unit, out var actor) == false)
                return;

            actor.animator.SetFloat("Forward", 2);
            actor.animator.SetTrigger("Move");
        }

        public void OnEndMove(Unit unit)
        {
            if (unitActorMaps.TryGetValue(unit, out var actor) == false)
                return;

            actor.animator.SetFloat("Forward", 0);
        }

        public void OnIdle(Unit unit)
        {
            if (unitActorMaps.TryGetValue(unit, out var actor) == false)
                return;

            actor.animator.SetTrigger("Idle");
        }

        public void OnStop(Unit unit)
        {
            if (unitActorMaps.TryGetValue(unit, out var actor) == false)
                return;

            actor.animator.SetTrigger("Idle");
        }

        public void OnClear(UnitActor actor)
        {
            DestroyImmediate(actor.gameObject);
        }

        public void OnFireProjectile(Unit me, Unit you, int projectileOriginID)
		{
            if (unitActorMaps.TryGetValue(me, out var actor) == false)
                return;

            var projectile = _projectilePool.GetProjectile(projectileOriginID, me, you);
            fireHistory.Add(projectile.projectileID, you);
            actor.animator.SetTrigger("Attack");
		}

        public void OnPositionChanging(Unit me, FixVector2 from, FixVector2 to)
        {

        }

        public void OnPositionChanged(Unit me, FixVector2 before, FixVector2 after)
        {
            if (unitActorMaps.TryGetValue(me, out var actor) == false)
                return;

            actor.transform.position = after;
        }

        public void OnUpdate(Unit me, Frame f)
        {
            if (unitActorMaps.TryGetValue(me, out var actor) == false)
                return;

            // float 캐스팅
            actor.animator.Update(f.deltaTime);
            actor.UpdateBounds();
        }

        public void OnHPChanging(Unit me, int from, int to)
        {
        }

        public void OnHPChanged(Unit me, int before, int after)
        {
            if (unitActorMaps.TryGetValue(me, out var actor) == false)
                return;

            actor.SetTintByHP(me.hp, me.maxhp);
        }

        public void OnSpawned(Unit me)
        {
            if (unitActorMaps.ContainsKey(me))
                return;

            var actor = _unitFactory.CreateNewUnit(me.type, _unitPrefabTable, null, this);
            unitActorMaps.Add(me, actor);
        }

        public void OnLookAt(Unit me, FixVector3 direction)
        {
            if (unitActorMaps.TryGetValue(me, out var actor) == false)
                return;

            actor.transform.LookAt(direction);
        }
    }
}
