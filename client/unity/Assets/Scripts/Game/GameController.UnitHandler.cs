using FixMath.NET;

namespace Game
{
    public partial class GameController : Unit.Listener
    {
        public void OnDead(Unit unit, Unit from)
        {
            unit.Die();
            unit.owner.units.Delete(unit);
        }

        public void OnOwnerChanged(Player before, Player after, Unit unit)
        {

        }

        public void OnAttack(Unit me, Unit you, Fix64 damage)
        {
            // TODO : Attack animation
        }

        public void OnDamaged(Unit me, Unit you, Fix64 damage)
        {
            // TODO : Damaged animation
            UnityEngine.Debug.Log($"hp : {me.hp}/{me.maxhp}");
        }

        public void OnHeal(Unit me, Unit you, Fix64 heal)
        {

        }
    }
}
