using System;
using System.Collections.Generic;
using System.Linq;
using FixMath.NET;
using KG;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "UnitTable", menuName = "Crash/UnitTable", order = 0)]
    public class UnitTable : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<int, Unit> _unitOriginDict;

        public Unit GetOriginUnit(int unitOriginID)
        {
            return _unitOriginDict[unitOriginID];
        }

        public void SetOriginUnit(int unitOriginID, Unit unit)
        {
            _unitOriginDict[unitOriginID] = unit;
        }

        public int AddNewUnit(Unit unit)
        {
            var nextKey = _unitOriginDict.Count > 0 ? _unitOriginDict.Keys.Max() + 1: 0;
            _unitOriginDict[nextKey] = unit;
            return unit.unitOriginID = nextKey;
        }

        public void RemoveKey(int unitOriginID)
        {
            _unitOriginDict.Remove(unitOriginID);
        }

        public void Clear()
        {
            _unitOriginDict.Clear();
        }

        public IEnumerable<KeyValuePair<int, Unit>> GetEnumerable()
        {
            return _unitOriginDict;
        }
    }
}