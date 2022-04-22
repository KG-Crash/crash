using UnityEngine;

namespace Game
{
    public class UnitActorFactory : MonoBehaviour
    {
        public UnitActor CreateUnitActor(int unitType, UnitTable unitTable, Transform parent, UnitActor.Listener listener)
        {
            var unitByType = unitTable.GetUnit(unitType);
            var unit = Object.Instantiate(unitByType, parent);
            unit.Init(listener);
            
            return unit;
        }
    }
}