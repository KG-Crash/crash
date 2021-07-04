using FixMath.NET;

namespace Game
{
    public partial class GameController : Unit.Listener
    {
        public void OnDead(Unit unit, Unit from)
        {
            unit.animator.SetTrigger("Dead");
        }

        public void OnOwnerChanged(Player before, Player after, Unit unit)
        {

        }

        public void OnAttack(Unit me, Unit you, Fix64 damage)
        {
            me.animator.SetTrigger("Attack");
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
            unit.animator.SetTrigger("Move");
        }

        public void OnEndMove(Unit unit)
        {
            unit.animator.SetTrigger("Idle");
        }

        public void OnStop(Unit unit)
        {
            unit.animator.SetTrigger("Idle");
        }
    }
}
