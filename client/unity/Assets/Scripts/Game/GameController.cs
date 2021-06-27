using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Protocol.Response;
using Shared;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [NonSerialized] private int _playerID;
        [NonSerialized] private uint _playerTeamID;
        [NonSerialized] private Team _allPlayerByTeam;
        [NonSerialized] private Player _player;
        [NonSerialized] private List<Unit> _selectedUnits;

        [SerializeField] private UnitTable _unitPrefabTable;
        
        private void Awake()
        {
            Handler.Instance.Bind(this);

            // 임시 코드 지워야함
            var units = new Unit[]
            {
                UnitFactory.GetNewUnit(0, 0,_unitPrefabTable),
                UnitFactory.GetNewUnit(1, 0,_unitPrefabTable),
                UnitFactory.GetNewUnit(2, 0, _unitPrefabTable)
            };

            units[0].transform.position = new Vector3(0, 0, -1.44f);
            units[1].transform.position = new Vector3(0, 0, 0);
            units[2].transform.position = new Vector3(0, 0, +1.44f);

            _playerID = 0;
            _playerTeamID = 0;
            _player = new Player();
            _player.AddUnits(units);
            _allPlayerByTeam = new Team();
            _allPlayerByTeam.players.Add(_playerTeamID, new List<Player>());
            _allPlayerByTeam.players[_playerTeamID].Add(_player);
            _selectedUnits = new List<Unit>();
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
                UnityObjects.IntersectUnits(ray, units, selectedUnits);
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

        public bool MoveSelectedUnitTo(Vector2 positionSS)
        {           
            var uobj = UnityResources._instance.Get("objects");
            var selectedCamera = uobj.GetCamera();
            var camRay = selectedCamera.ScreenPointToRay(positionSS);

            if (Physics.Raycast(camRay, out var hitInfo, selectedCamera.farClipPlane))
            {
                foreach (var unit in _selectedUnits)
                {
                    unit.MoveTo(CrashMath.EncodePosition(hitInfo.point));
                }

                return true;
            }
            else
            {
                return false;
            }
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
    }
}
