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

            foreach (var projectile in unit.projectiles.Values)
                projectile.owner = null;
            
            if (unitActorMaps.TryGetValue(unit, out var x) == false)
                return;

            var actor = x as UnitActor;
            actor.animator.SetTrigger("Dead");
            unit.Destroy();
        }

        public void OnRemove(LogicalObject unit)
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
            if (unitActorMaps.TryGetValue(me, out var x) == false)
                return;

            var myActor = x as UnitActor;
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
            if (unitActorMaps.TryGetValue(unit, out var x) == false)
                return;

            var actor = x as UnitActor;
            actor.animator.SetFloat("Forward", 2);
            actor.animator.SetTrigger("Move");
        }

        public void OnEndMove(Unit unit)
        {
            if (unitActorMaps.TryGetValue(unit, out var x) == false)
                return;

            var actor = x as UnitActor;
            actor.animator.SetFloat("Forward", 0);
        }

        public void OnIdle(Unit unit)
        {
            if (unitActorMaps.TryGetValue(unit, out var x) == false)
                return;

            var actor = x as UnitActor;
            actor.animator.SetTrigger("Idle");
        }

        public void OnStop(Unit unit)
        {
            if (unitActorMaps.TryGetValue(unit, out var x) == false)
                return;

            var actor = x as UnitActor;
            actor.animator.SetTrigger("Idle");
        }

        public void OnClear(UnitActor actor)
        {
            DestroyImmediate(actor.gameObject);
        }

        // TODO :: 무지성 시퀀스 어케함?
        public static uint SEQUENCE = 0; 
        
        public void OnFireProjectile(Unit fireUnit, Unit target, int projectileOriginID)
		{
            if (unitActorMaps.TryGetValue(fireUnit, out var x) == false)
                return;

            var actor = x as UnitActor;
            actor.animator.SetTrigger("Attack");
            
            // TODO :: Projectile 오브젝트까지 풀링을 해야하는가?
            var projectile = fireUnit.projectiles.Add(SEQUENCE++, target);
            _fireHistory.Add(projectile, target);
		}

        public void OnPositionChanging(LogicalObject me, FixVector2 from, FixVector2 to)
        {
        }

        public void OnPositionChanged(LogicalObject me, FixVector2 before, FixVector2 after)
        {
            if (unitActorMaps.TryGetValue(me, out var x) == false)
                return;

            x.position = after;
        }

        public void OnUpdate(Unit me, Frame f)
        {
            if (unitActorMaps.TryGetValue(me, out var x) == false)
                return;

            var actor = x as UnitActor;
            // float 캐스팅
            actor.animator.Update(f.deltaTime);
            actor.UpdateBounds();
        }

        public void OnHPChanging(Unit me, int from, int to)
        {
        }

        public void OnHPChanged(Unit me, int before, int after)
        {
            if (unitActorMaps.TryGetValue(me, out var x) == false)
                return;

            var actor = x as UnitActor;
            actor.SetTintByHP(me.hp, me.maxhp);
        }

        public void OnSpawned(Unit me)
        {
            if (unitActorMaps.ContainsKey(me))
                return;

            var actor = unitActorFactory.CreateUnitActor(me.type, _unitPrefabTable, null, this);
            unitActorMaps.Add(me, actor);
        }

        public void OnLookAt(LogicalObject me, FixVector3 direction)
        {
            if (unitActorMaps.TryGetValue(me, out var actor) == false)
                return;

            actor.LookAt(direction);
        }
    }
}
