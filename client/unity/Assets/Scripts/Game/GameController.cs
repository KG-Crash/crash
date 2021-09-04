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

        [SerializeField] private Transform _focusTransform;
        
        [SerializeField] private KG.Map _map;
        [SerializeField] private UnitFactory _unitFactory;
        [SerializeField] private UnitTable _unitPrefabTable;

        [Header("Debug")]
        public Transform spawnMine;
        public Transform spawnEnemy;
        
        private static uint playerIDStepper = 0;

        private Player AddNewPlayer(uint teamID, int spawnIndex)
        {
            var newPlayer = new Player(playerIDStepper++, teamID, this);
            _allPlayerByTeam.AddPlayer(teamID, newPlayer);
            newPlayer.spawnIndex = spawnIndex;
            
            return newPlayer;
        }

        private Unit SpawnUnitToPlayerStart(int spawnUnitOriginID, Player ownPlayer)
        {
            var newUnit = _unitFactory.GetNewUnit(_map, spawnUnitOriginID, ownPlayer.teamID, _unitPrefabTable, this);
            ownPlayer.units.Add(newUnit);
            newUnit.position = _map.GetSpawnPosition(ownPlayer.spawnIndex);

            return newUnit;
        }

        /*
         * 유닛 스폰 및 해당 맵의 첫번째 처리 루틴,
         * 지금은 임시 코드가 들어감.
         */
        private void OnLoadScene()
        {   
            _allPlayerByTeam = new Team();
            
            var player = AddNewPlayer(0, 0);
            
            SpawnUnitToPlayerStart(0, player);
            SpawnUnitToPlayerStart(1, player);
            SpawnUnitToPlayerStart(2, player);

            var otherPlayer = AddNewPlayer(1, 1);
            
            SpawnUnitToPlayerStart(0, otherPlayer);
            SpawnUnitToPlayerStart(1, otherPlayer);

            _player = player;

            if (_player.units.Any())
            {
                var sum = _player.units.Aggregate(FixVector3.Zero, (acc, x) => acc + x.position);
                var lookPosition = sum / _player.units.Count();
                _focusTransform.position = lookPosition;
            }
        }
        
        private void Awake()
        {
            Handler.Instance.Bind(this);
            
            OnLoadScene();
            
            _selectedUnits = new List<Unit>();
            _allUnitInFrustum = new List<Unit>();
        }

        private void Update()
        {
            _player.UpdateUpgrade(UnityEngine.Time.time);
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
