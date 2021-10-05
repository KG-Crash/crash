using UnityEngine;

namespace Game
{
    public class UnitFactory : MonoBehaviour
    {
        private uint _sequence = 0;
        
        public Unit GetNewUnit(int unitOriginID, UnitTable unitTable, KG.Map map, Player owner, Unit.Listener listener)
        {
            var unitOrigin = unitTable.GetOriginUnit(unitOriginID);
            var unit = Object.Instantiate(unitOrigin);
            unit.Init(_sequence++, map, owner, listener);
            unit.SetMaxHP();
            return unit;
        }
    }
}