using System;
using Shared;
using UnityEngine;

namespace Game
{
    public interface ICommand
    {
        CommandType type { get; }
        uint behaiveUnitID { get;  }
    }

    public static class CommandFactory
    {
        public static ICommand CreateMoveCommand(uint unitID, Vector3Int pos)
        {
            return new MoveCommand(unitID, pos);
        }
        public static ICommand CreateAttackSingleCommand(uint unitID, uint targetUnitID)
        {
            return new AttackSingleCommand(unitID, targetUnitID);
        }
        public static ICommand CreateAttackMultiCommand(uint unitID, uint[] targetUnitIDs)
        {
            return new AttackMultiCommand(unitID, targetUnitIDs);
        }
    }
    
    [System.Serializable]
    public struct MoveCommand : ICommand
    { 
        public CommandType type => CommandType.Move;
        public uint behaiveUnitID { get; set; }
        public Vector3Int _position;
        
        public MoveCommand(uint behaiveUnitID, Vector3Int position)
        {
            this.behaiveUnitID = behaiveUnitID;
            _position = position;
        }
    }

    [System.Serializable]
    public struct AttackSingleCommand : ICommand
    {
        public CommandType type => CommandType.AttackSingleTarget;
        public uint behaiveUnitID { get; set; }
        public uint _targetUnitID;
        
        public AttackSingleCommand(uint behaiveUnitID, uint targetUnitID)
        {
            this.behaiveUnitID = behaiveUnitID;
            _targetUnitID = targetUnitID;
        }
        
    }

    [System.Serializable]
    public struct AttackMultiCommand : ICommand 
    {        
        public CommandType type => CommandType.AttackMultiTarget;
        public uint behaiveUnitID { get; set; }
        public uint[] _targetUnitIDs;
        
        public AttackMultiCommand(uint behaiveUnitID, uint[] targetUnitIDs)
        {
            this.behaiveUnitID = behaiveUnitID;
            _targetUnitIDs = new uint[targetUnitIDs.Length];
            Array.Copy(targetUnitIDs, _targetUnitIDs, targetUnitIDs.Length);
        }
    }
}