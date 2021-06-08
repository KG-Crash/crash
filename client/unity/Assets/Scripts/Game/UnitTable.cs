using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "UnitTable", menuName = "Crash/UnitTable", order = 0)]
    public class UnitTable : ScriptableObject
    {
        [SerializeField] private Unit[] _unitOrigin;

        public Unit GetOrigin(int unitOriginID)
        {
            return Array.Find(_unitOrigin, unit => unit.unitOriginID == unitOriginID);
        }
    }
}