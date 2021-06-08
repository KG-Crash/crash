using UnityEngine;

namespace Game
{
    public static class UnitFactory 
    {
        public static Unit GetNewUnit(int unitOriginID, UnitTable unitTable)
        {
            var unitOrigin = unitTable.GetOrigin(unitOriginID);
            return Object.Instantiate(unitOrigin);
        }
    }
}