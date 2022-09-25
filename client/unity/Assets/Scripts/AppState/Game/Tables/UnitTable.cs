using KG;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "UnitTable", menuName = "Crash/UnitTable", order = 0)]
    public class UnitTable : UnityTable<UnitTable>
    {
        [FormerlySerializedAs("_unitOriginDict")] [SerializeField] private SerializableDictionary<int, UnitActor> _unitTypeDict;

        public UnitActor GetUnit(int unitType)
        {
            return _unitTypeDict[unitType];
        }

        public void SetUnit(int unitType, UnitActor unit)
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

        public IEnumerable<KeyValuePair<int, UnitActor>> GetEnumerable()
        {
            return _unitTypeDict;
        }
    }
}