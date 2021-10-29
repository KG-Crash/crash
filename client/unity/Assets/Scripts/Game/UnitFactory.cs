using UnityEngine;

namespace Game
{
    public class UnitFactory : MonoBehaviour
    {
        private uint _sequence = 0;
        
        public Unit CreateNewUnit(int unitOriginID, UnitTable unitTable, KG.Map map, Player owner, Unit.Listener listener, Transform parent)
        {
            var unitOrigin = unitTable.GetOriginUnit(unitOriginID);
            var unit = Object.Instantiate(unitOrigin, parent);
            unit.Init(_sequence++, map, owner, listener);
            return unit;
        }
    }
}