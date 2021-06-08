using System.Runtime.InteropServices;
using UnityEngine;

namespace Network
{
    public enum ClientAction
    {
        Move = 0,
        AttackSingle = 1,
        AttackMulti = 2,
        NewUnit = 3,
        DeathUnit = 4,
    }
    
    [System.Serializable]
    public struct Area
    {
        public UnityEngine.Vector3 _min;
        public UnityEngine.Vector3 _max;
    }
    
    [System.Serializable]
    public struct Command
    { 
        public ClientAction _action;
        public int _originUnitID;
        public int _targetID;
        public Area _area;
        public Vector3 _moveTo;
    }
}