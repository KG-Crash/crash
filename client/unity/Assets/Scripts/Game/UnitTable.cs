using System;
using System.Collections.Generic;
using System.Linq;
using FixMath.NET;
using KG;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "UnitTable", menuName = "Crash/UnitTable", order = 0)]
    public class UnitTable : ScriptableObject
    {
        [FormerlySerializedAs("_unitOriginDict")] [SerializeField] private SerializableDictionary<int, Unit> _unitTypeDict;

        public Unit GetUnitByType(int unitType)
        {
            return _unitTypeDict[unitType];
        }

        public void SetUnitByType(int unitType, Unit unit)
        {
            _unitTypeDict[unitType] = unit;
        }

        public int GetNewKey()
        {
            return _unitTypeDict.Count > 0 ? _unitTypeDict.Keys.Max() + 1: 0;
        }
        
        public void RemoveKey(int unitType)
        {
            _unitTypeDict.Remove(unitType);
        }

        public void Clear()
        {
            _unitTypeDict.Clear();
        }

        public IEnumerable<KeyValuePair<int, Unit>> GetEnumerable()
        {
            return _unitTypeDict;
        }
    }
}