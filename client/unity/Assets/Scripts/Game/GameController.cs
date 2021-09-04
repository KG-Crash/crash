using Network;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Table;
using UnityEngine;

namespace Game
{
    public partial class GameController : MonoBehaviour
    {
        [NonSerialized] private int _playerID;
        [NonSerialized] private uint _playerTeamID;
        [NonSerialized] private Team _allPlayerByTeam;
        [NonSerialized] private Player _player;
        [NonSerialized] private List<Unit> _selectedUnits;
        [NonSerialized] private List<Unit> _allUnitInFrustum;

        [SerializeField] private KG.Map _map;
        [SerializeField] private UnitFactory _unitFactory;
        [SerializeField] private UnitTable _unitPrefabTable;

        [Header("Debug")]
        public Transform spawnMine;
        public Transform spawnEnemy;
        
        private void Awake()
        {
            Handler.Instance.Bind(this);

            var firstCell = _map.cells.FirstOrDefault(x => x.data.walkable);
            if (firstCell == null)
                return;

            // 임시 코드 지워야함
            var units = new Unit[]
            {
                _unitFactory.GetNewUnit(_map, 0, 0, _unitPrefabTable, this),
                _unitFactory.GetNewUnit(_map, 1, 0, _unitPrefabTable, this),
                _unitFactory.GetNewUnit(_map, 2, 0, _unitPrefabTable, this)
            };

            units[0].position = spawnMine.position;
            units[1].position = spawnMine.position;
            units[2].position = spawnMine.position;

            var end = _map.cells.OrderBy(x => x.data.row + x.data.col).LastOrDefault(x => x.data.walkable);
            if (end == null)
                return;

            units[0].MoveTo(end.data.position);
            units[2].MoveTo(end.data.position);

            _playerID = 0;
            _playerTeamID = 0;
            _player = new Player();
            _player.units.AddRange(units);

            _allPlayerByTeam = new Team();
            _allPlayerByTeam.players.Add(_playerTeamID, new List<Player>());
            _allPlayerByTeam.players[_playerTeamID].Add(_player);

            var enemyUnits = new Unit[]
            {
                _unitFactory.GetNewUnit(_map, 0, 1, _unitPrefabTable, this),
                //_unitFactory.GetNewUnit(_map, 1, 1, _unitPrefabTable, this),
            };

            enemyUnits[0].position = spawnEnemy.position;
            //enemyUnits[1].position = new Vector3(firstCell.data.position.x, 0, firstCell.data.position.y);

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
            UpdateUpgrade(UnityEngine.Time.time);
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
                
                if (selectedUnit)
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
            else if (camRay.origin.y > 0 && camRay.direction.y < 0)
            {
                var t = camRay.origin.y / (-camRay.direction.y);
                var point = camRay.GetPoint(t);
             
                foreach (var unit in _selectedUnits)
                {
                    unit.MoveTo(point);
                }
                    
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Unit> GetAllUnits()
        {
            return _allPlayerByTeam.players.SelectMany(x => x.Value).SelectMany(x => x.units).Select(x => x);
        }

        public void UpdateUnitInFrustumPlane()
        {
            var uobj = UnityResources._instance.Get("objects");
            var selectedCamera = uobj.GetCamera();
            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(selectedCamera);
            
            _allUnitInFrustum = GetAllUnits().
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

    }
}
