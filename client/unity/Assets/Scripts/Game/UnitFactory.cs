using UnityEngine;

namespace Game
{
    public class UnitFactory : MonoBehaviour
    {
        private uint _sequence = 0;
        
        public Unit GetNewUnit(KG.Map map, int unitOriginID, uint teamID, UnitTable unitTable, Unit.Listener listener)
        {
            var unitOrigin = unitTable.GetOrigin(unitOriginID);
            var unit = Object.Instantiate(unitOrigin);
            unit.unitOriginID = unitOriginID;
            unit.unitID = _sequence++;
            unit.teamID = teamID;
            unit.listener = listener;
            unit.map = map;
            return unit;
        }
    }
}