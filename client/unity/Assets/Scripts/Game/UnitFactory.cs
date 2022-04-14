using UnityEngine;

namespace Game
{
    public class UnitFactory : MonoBehaviour
    {
        private uint _sequence = 0;
        
        public UnitActor CreateNewUnit(int unitType, UnitTable unitTable, Transform parent, UnitActor.Listener listener)
        {
            var unitByType = unitTable.GetUnitByType(unitType);
            var unit = Object.Instantiate(unitByType, parent);
            unit.Init(listener);
            
            return unit;
        }
    }
}