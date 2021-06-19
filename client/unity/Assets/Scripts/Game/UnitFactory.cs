using UnityEngine;

namespace Game
{
    public static class UnitFactory
    {
        private static uint _unitIDStepper = 0;
        
        public static Unit GetNewUnit(int unitOriginID, uint teamID, UnitTable unitTable)
        {
            var unitOrigin = unitTable.GetOrigin(unitOriginID);
            var unit = Object.Instantiate(unitOrigin);
            unit.unitID = _unitIDStepper++;
            unit.teamID = teamID;
            return unit;
        }
    }
}