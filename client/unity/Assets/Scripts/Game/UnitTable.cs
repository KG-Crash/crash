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

        public int GetNewKey()
        {
            return _unitOriginDict.Count > 0 ? _unitOriginDict.Keys.Max() + 1: 0;
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