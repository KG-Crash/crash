using FixMath.NET;
using Module;
using Network;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Protocol.Request;
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
        
        public static int InputFrame { get; private set; }
        public static int InputTurn { get; private set; }
        public static int InputTotalFrame => InputFrame + InputTurn * Shared.Const.Time.FramePerTurn;

        public static Frame InputFrameChunk => new Frame()
            {currentFrame = InputFrame, currentTurn = InputTurn, deltaTime = TimeDelta};
        
        public static int OutputFrame { get; private set; }
        public static int OutputTurn { get; private set; }
        public static int OutputTotalFrame => OutputFrame + OutputTurn * Shared.Const.Time.FramePerTurn;

        public static Frame OutputFrameChunk => new Frame()
            {currentFrame = OutputFrame, currentTurn = OutputTurn, deltaTime = TimeDelta};

        private readonly Protocol.Request.ActionQueue _actionQueue = new Protocol.Request.ActionQueue 
        {
            Actions = new List<Protocol.Request.Action>()
        };

        [NonSerialized] private int _playerID;
        [NonSerialized] private uint _playerTeamID;
        [NonSerialized] private Team _allPlayerByTeam;
        [NonSerialized] private Player _player;
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

            // 레디에서 이름 보내야 하지 않을까?
            _ = Client.Send(new Protocol.Request.Ready{ });

            InitializeUniRx();
        }

        private void Start()
        {
            Application.targetFrameRate = FPS;
        }

        private void OnUpdateAlways()
        {
            OnUpdateAlwaysDebug();
            
            if (_actions.Count > 0 && _actions.All(kv =>
            {
                var list = kv.Value;
                if (list.Count == 0)
                {
                    return false;
                }

                return list.First.Value.Turn == OutputTurn;
            }))
            {
                foreach (var kv in _actions)
                {
                    var list = kv.Value;
                    OnUpdateAction(list.First.Value);
                    list.RemoveFirst();
                }

                OutputTurn++;
            }
            
            Debug.Log($"InputTurn({InputTurn}) > OutputTurn({OutputTurn}) + 2");
            paused = InputTurn > OutputTurn + 2;
        }

        private void OnUpdateAction(Protocol.Response.ActionQueue queue)
        {
            
        }

        private void OnUpdateFrame(Frame f)
        {
            Debug.Log($"InputTurn({InputTurn}) > OutputTurn({OutputTurn}) + 2");
            
            EnqueueAction(new Protocol.Request.Action
            {
                Frame = InputFrame,
                Id = 0,
                Param1 = (uint)(TPS - InputFrame),
                Param2 = (uint)InputFrame
            });
            
            _player.UpdateUpgrade(f);
            OnUpdateFrameDebug(f);
        }

        private void OnLateUpdateFrame(Frame f)
        {
            Debug.Log($"InputTurn({InputTurn}) > OutputTurn({OutputTurn}) + 2");
            
            if (++InputFrame >= Shared.Const.Time.FramePerTurn)
            {
                OnTurnChanged(InputTurn++);
                InputFrame = 0;
            }
            
            paused = InputTurn > OutputTurn + 2;
        }

        private void OnDestroy()
        {
            ClearInput();
        }

        private void OnTurnChanged(int turn)
        {
            if (IsNetworkMode)
            {
                _actionQueue.Turn = turn;
                _ = Client.Send(_actionQueue);
                Debug.Log($"send queue : {turn}");
            }
            _actionQueue.Actions.Clear();
        }

        public void EnqueueAction(Protocol.Request.Action action)
        {
            action.Frame = InputFrame;
            _actionQueue.Actions.Add(action);
        }
    }
}
