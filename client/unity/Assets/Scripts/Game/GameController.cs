using Network;
using Protocol.Response;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour, IUnit
    {
        [NonSerialized] private int _playerID;
        [NonSerialized] private uint _playerTeamID;
        [NonSerialized] private Team _allPlayerByTeam;
        [NonSerialized] private Player _player;
        [NonSerialized] private List<Unit> _selectedUnits;
        [NonSerialized] private List<Unit> _allUnitInFrustum;
        
        [SerializeField] private UnitTable _unitPrefabTable;
        private void Awake()
        {
            Handler.Instance.Bind(this);

            // 임시 코드 지워야함
            var units = new Unit[]
            {
                UnitFactory.GetNewUnit(0, 0, _unitPrefabTable, this),
                UnitFactory.GetNewUnit(1, 0, _unitPrefabTable, this),
                UnitFactory.GetNewUnit(2, 0, _unitPrefabTable, this)
            };

            units[0].transform.position = new Vector3(0, 0, -1.44f);
            units[1].transform.position = new Vector3(0, 0, 0);
            units[2].transform.position = new Vector3(0, 0, +1.44f);

            _playerID = 0;
            _playerTeamID = 0;
            _player = new Player();
            _player.units.AddRange(units);

            _allPlayerByTeam = new Team();
            _allPlayerByTeam.players.Add(_playerTeamID, new List<Player>());
            _allPlayerByTeam.players[_playerTeamID].Add(_player);

            var enemyUnits = new Unit[]
            {
                UnitFactory.GetNewUnit(0, 1, _unitPrefabTable, this),
                UnitFactory.GetNewUnit(1, 1, _unitPrefabTable, this),
            };
            
            enemyUnits[0].transform.position = new Vector3(-1.44f * -1, 0, 0);
            enemyUnits[1].transform.position = new Vector3(-1.44f * 2, 0, 0);

            var otherPlayer = new Player();
            otherPlayer.units.AddRange(enemyUnits);
            uint otherPlayerID = 1;
            _allPlayerByTeam.players.Add(otherPlayerID, new List<Player>());
            _allPlayerByTeam.players[otherPlayerID].Add(otherPlayer);
            
            _selectedUnits = new List<Unit>();
            _allUnitInFrustum = new List<Unit>();
        }

        private void Update()
        {            
            UpdateUnitInFrustumPlane();
        }

        private void OnDestroy()
        {
            
        }

        private Vector3[] frustumPoints = new Vector3[8];
        private Plane[] frustumPlanes = new Plane[6]; 
        
        public void UpdateDragRect(Rect rect)
        {
            var uobj = UnityResources._instance.Get("objects");
            var selectedCamera = uobj.GetCamera();
            var teamPlayers = _allPlayerByTeam.players[_playerTeamID];
            var player = teamPlayers[_playerID];
            var units = player.units.Values.ToArray();
            var selectedUnits = new List<Unit>();

            if (rect.size != Vector2.zero)
            {
                CrashMath.GetFrustumPlanes(selectedCamera, rect, frustumPlanes); 
                UnityObjects.IntersectUnits(frustumPlanes, units, selectedUnits);
            }
            else
            {
                var ray = selectedCamera.ScreenPointToRay(rect.position);
                var selectedUnit = UnityObjects.IntersectNearestUnit(ray, units);
                selectedUnits.Add(selectedUnit);
            }
            
            SelectUnits(selectedUnits);
        }

        public void SelectUnits(List<Unit> selectedUnitList)
        {
            foreach (var unit in _selectedUnits.Except(selectedUnitList))
            {
                unit.Selected(false);
            }
            foreach (var unit in selectedUnitList.Except(_selectedUnits))
            {
                unit.Selected(true);
            }

            _selectedUnits = selectedUnitList;
        }

        public void MoveOrAttackTo(Vector2 positionSS, out bool move)
        {
            var uobj = UnityResources._instance.Get("objects");
            var selectedCamera = uobj.GetCamera();

            var ray = selectedCamera.ScreenPointToRay(positionSS);
            var selectedUnit = UnityObjects.IntersectNearestUnit(ray, _allUnitInFrustum);
            
            if (selectedUnit != null)
            {
                if (selectedUnit.teamID == _playerTeamID)
                {
                    MoveSelectedUnitTo(selectedUnit);
                    move = true;
                }
                else
                {
                    AttackSelectedUnitTo(selectedUnit);
                    move = false;
                }
            }
            else
            {
                MoveSelectedUnitTo(ray, selectedCamera.farClipPlane);
                move = true;
            }
        }
        
        public bool MoveSelectedUnitTo(Ray camRay, float farClipPlane)
        {           
            if (Physics.Raycast(camRay, out var hitInfo, farClipPlane))
            {
                foreach (var unit in _selectedUnits)
                {
                    unit.MoveTo(hitInfo.point);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void MoveSelectedUnitTo(Unit moveTargetUnit)
        {                    
            foreach (var unit in _selectedUnits)
            {
                unit.MoveTo(moveTargetUnit);
            }
        }

        public void AttackSelectedUnitTo(Unit attackTargetUnit)
        {
            Debug.Log($"{attackTargetUnit.name}, {attackTargetUnit.teamID}");
            
            foreach (var unit in _selectedUnits)
            {
                unit.AttackTo(attackTargetUnit);
            }
        }

        public void UpdateUnitInFrustumPlane()
        {
            var uobj = UnityResources._instance.Get("objects");
            var selectedCamera = uobj.GetCamera();
            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(selectedCamera);
            
            _allUnitInFrustum = _allPlayerByTeam.players.
                SelectMany(x => x.Value).
                SelectMany(x => x.units).
                Select(x => x).
                Where(x => GeometryUtility.TestPlanesAABB(frustumPlanes, x.bounds)).ToList();
        }

        public void ApplyCommand(IEnumerable<ICommand> commands)
        {
            var teamPlayers = _allPlayerByTeam.players[_playerTeamID];
            var player = teamPlayers[_playerID];
            
            foreach (var command in commands)
            {
                switch (command.type)
                {
                    case CommandType.Move:
                        var unit = player.units[command.behaiveUnitID];
                        var moveCommand = (MoveCommand) command;
                        unit.MoveTo(CrashMath.DecodePosition(moveCommand._position));
                        break;
                    case CommandType.AttackSingleTarget:
                        break;
                    case CommandType.AttackMultiTarget:
                        break;
                }
            }
        }

        public void AppendCommand(ICommand command)
        {
            ApplyCommand(new ICommand[1] { command });
        }

        public void AppendCommand(IEnumerable<ICommand> commands)
        {
            ApplyCommand(commands);
        }

        [FlatBufferEvent]
        public bool OnCreateRoom(CreateRoom response)
        {
            return true;
        }

        [FlatBufferEvent]
        public bool OnJoinRoom(JoinRoom response)
        {
            return true;
        }

        public void OnDead(Unit unit)
        {
            unit.Die();
            unit.owner.units.Delete(unit);
        }

        public void OnOwnerChanged(Player owner, Unit unit)
        {
            unit.ChangeOwner(owner);
        }
    }
}
