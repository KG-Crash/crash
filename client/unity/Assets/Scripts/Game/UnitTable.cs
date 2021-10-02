using System;
using FixMath.NET;
using KG;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "UnitTable", menuName = "Crash/UnitTable", order = 0)]
    public class UnitTable : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<int, Unit> _unitOriginDict;

        public Unit GetOrigin(int unitOriginID)
        {
            return _unitOriginDict[unitOriginID];
        }
    }
}