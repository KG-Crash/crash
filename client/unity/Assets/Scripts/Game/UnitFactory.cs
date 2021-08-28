using UnityEngine;

namespace Game
{
    public class UnitFactory : MonoBehaviour
    {
        [SerializeField] private KG.Map _map;
        private uint _sequence = 0;
        
        public Unit GetNewUnit(int unitOriginID, uint teamID, UnitTable unitTable, Unit.Listener listener)
        {
            var unitOrigin = unitTable.GetOrigin(unitOriginID);
            var unit = Object.Instantiate(unitOrigin);
            unit.unitID = _sequence++;
            unit.teamID = teamID;
            unit.listener = listener;
            unit.map = _map;
            return unit;
        }
    }
}