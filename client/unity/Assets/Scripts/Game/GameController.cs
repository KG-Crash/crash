using FixMath.NET;
using Module;
using Network;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public partial class GameController : MonoBehaviour
    {   
        public static int FPS { get; set; }
        public static int TPS { get; set; }
        public static bool IsNetworkMode { get; set; }
        public static Fix64 TimeSpeed { get; set; } = Fix64.One;
        public static Fix64 TimeDelta => (Fix64.One * TimeSpeed) / new Fix64(FPS);
        public static Fix64 TurnRate => Fix64.One / new Fix64(8);
        public static bool paused { get; set; }
        
        public static int Frame { get; private set; }
        public static int Turn { get; private set; }
        public static int TotalFrame => Frame + Turn * Shared.Const.Time.FramePerTurn;
        
        private readonly Protocol.Request.ActionQueue _actionQueue = new Protocol.Request.ActionQueue 
        {
            Actions = new List<Protocol.Request.Action>()
        };

        [NonSerialized] private int _playerID;
        [NonSerialized] private uint _playerTeamID;
        [NonSerialized] private Team _allPlayerByTeam;
        [NonSerialized] private Player _player;
        [NonSerialized] private List<Unit> _selectedUnits;
        [NonSerialized] private List<Unit> _allUnitInFrustum;
        [NonSerialized] private ProjectilePool _projectilePool;

        [Header("Miscellaneous")] 
        [SerializeField] private Transform _poolOffset; 
        [SerializeField] private Transform _focusTransform;
        
        [SerializeField] private KG.Map _map;
        [SerializeField] private UnitFactory _unitFactory;
        [SerializeField] private UnitTable _unitPrefabTable;
        [SerializeField] private ProjectileTable _projectilehPrefabTable;
        [SerializeField] private bool _networkMode;
       
        private static uint playerIDStepper = 0;

        public Player GetPlayer(uint playerID)
        {
            return _allPlayerByTeam.GetPlayer(playerID);
        }
        
        private Player AddNewPlayer(uint teamID, int spawnIndex)
        {
            var newPlayer = new Player(playerIDStepper++, teamID, this);
            _allPlayerByTeam.AddPlayer(teamID, newPlayer);
            newPlayer.spawnIndex = spawnIndex;
            
            return newPlayer;
        }

        private void Awake()
        {
            Handler.Bind(this, Dispatcher.Instance);

            FPS = Shared.Const.Time.FPS;
            TPS = Shared.Const.Time.TPS;
            IsNetworkMode = _networkMode;

            Application.targetFrameRate = FPS;
            
            InitInput();
            OnLoadScene();
            
            _projectilePool = new ProjectilePool(_projectilehPrefabTable, 15, this, _poolOffset);
            _selectedUnits = new List<Unit>();
            _allUnitInFrustum = new List<Unit>();

            _ = Client.Send(new Protocol.Request.Ready{ });

            InitializeUniRx();
        }

        private void Start()
        {
            Application.targetFrameRate = FPS;
        }

        private void OnUpdateAlways()
        {
            UpdateUnitInFrustumPlane();
            UpdateForDebug();
        }

        private void OnUpdateFrame(Fix64 timeDelta)
        {
            EnqueueAction(new Protocol.Request.Action
            {
                Frame = Frame,
                Id = 0,
                Param1 = (uint)(TPS - Frame),
                Param2 = (uint)Frame
            });

            if (++Frame >= Shared.Const.Time.FramePerTurn)
            {
                OnTurnChanged(Turn++);
                Frame = 0;
            }
            
            _player.UpdateUpgrade();
        }

        private void OnDestroy()
        {
            ClearInput();
        }

        private Vector3[] frustumPoints = new Vector3[8];
        private Plane[] frustumPlanes = new Plane[6];

        private void OnTurnChanged(int turn)
        {
            if (IsNetworkMode)
            {
                _actionQueue.Turn = turn;
                _ = Client.Send(_actionQueue);
            }
            _actionQueue.Actions.Clear();
        }

        public void EnqueueAction(Protocol.Request.Action action)
        {
            action.Frame = Frame;
            _actionQueue.Actions.Add(action);
        }
        
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
