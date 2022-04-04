using FixMath.NET;
using Module;
using Network;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game
{
    public static class ActionExtension
    {
        public static ushort LOWORD(this uint actionParam) => (ushort)(actionParam & 0x0000FFFF);
        public static ushort HIWORD(this uint actionParam) => (ushort)((actionParam & 0xFFFF0000) >> 8);

        public static uint SetParam1LOWORD(this Protocol.Request.Action action, ushort value)
        {
            action.Param1 = (action.Param1 & 0xFFFF0000) + (uint)(value & 0x0000FFFF);
            return action.Param1;
        }

        public static uint SetParam1HIWORD(this Protocol.Request.Action action, ushort value)
        {
            action.Param1 = (action.Param1 & 0x0000FFFF) + (uint)((value & 0x0000FFFF) << 16);
            return action.Param1;
        }

        public static uint SetParam2LOWORD(this Protocol.Request.Action action, ushort value)
        {
            action.Param2 = (action.Param2 & 0xFFFF0000) + (uint)(value & 0x0000FFFF);
            return action.Param2;
        }

        public static uint SetParam2HIWORD(this Protocol.Request.Action action, ushort value)
        {
            action.Param2 = (action.Param2 & 0x0000FFFF) + (uint)((value & 0x0000FFFF) << 16);
            return action.Param2;
        }
    }

    public partial class GameController : MonoBehaviour
    {   
        public static int FPS { get; set; }
        public static int TPS { get; set; }
        public static bool IsNetworkMode { get; set; }
        public static Fix64 TimeSpeed { get; set; } = Fix64.One;
        public static Fix64 TimeDelta => (Fix64.One * TimeSpeed) / new Fix64(FPS);
        public static Fix64 TurnRate => Fix64.One / new Fix64(8);
        public static bool paused { get; set; }
        public static bool ready { get; set; }
        public static bool waitPacket { get; set; }
        
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

        private Dictionary<ActionKind, MethodInfo[]> _actionHandleMethodDict;
        private ActionHandleParam _actionHandleParam;
        private object[] _actionMethodParam;

        [NonSerialized] private int _playerID;
        [NonSerialized] private uint _playerTeamID;
        [NonSerialized] private Team _allPlayerByTeam;
        [NonSerialized] private Player _player;
        [NonSerialized] private ProjectilePool _projectilePool;
        [NonSerialized] private ChatManager _chatManager;

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
            _chatManager = this.gameObject.GetComponent<ChatManager>();

            // 레디에서 이름 보내야 하지 않을까?
            _ = Client.Send(new Protocol.Request.Ready{ });

            _actionHandleMethodDict = MethodExtractor.ExtractActionHandleMethod<GameController>();
            _actionHandleParam = new ActionHandleParam();
            _actionMethodParam = new object[2];
            
            InitializeUniRx();
        }

        private void Start()
        {
            Application.targetFrameRate = FPS;
        }

        private void OnUpdate()
        {
            OnUpdateAlwaysDebug();

            if (!ready || paused)
            {
                return;
            }
            
            Debug.Log($"InputTurn({InputTurn}) > OutputTurn({OutputTurn}) + 2");
            waitPacket = InputTurn > OutputTurn + 2;

            var buffers = _actions.Pop(OutputTurn);
            if (buffers == null)
                return;

            foreach (var pair in buffers)
            {
                var userId = pair.Key;
                var buffer = pair.Value;

                var protocols = buffer.Protocols.GroupBy(x => x.GetType())
                    .ToDictionary(x => x.Key, x => x.ToList());

                if(protocols.TryGetValue(typeof(Protocol.Response.Action), out var actions) == false)
                {
                    actions = new List<Protocol.IProtocol>();
                }
                if(protocols.TryGetValue(typeof(Protocol.Response.InGameChat), out var chats) == false)
                {
                    chats = new List<Protocol.IProtocol>();
                }
                foreach(var action in actions.Select(x => x as Protocol.Response.Action))
                {
                    OnUpdateAction(userId, action);
                }
                foreach (var chat in chats.Select(x => x as Protocol.Response.InGameChat))
                {
                    OnUpdateIngameChat(userId, chat);
                }
            }

            OutputTurn++;
        }

        private void OnUpdateAction(int userId, Protocol.Response.Action action)
        {
            var actionKind = (ActionKind)action.Id;
            var methods = _actionHandleMethodDict[actionKind];

            _actionHandleParam.userId = userId;
            
            _actionMethodParam[0] = action;
            _actionMethodParam[1] = _actionHandleParam; 
            
            for (var i = 0; i < methods.Length; i++)
            {
                methods[i].Invoke(this, _actionMethodParam);
            }
            
            Debug.Assert(methods.Length > 0, $"{actionKind} 처리 메소드가 없음");
        }

        private void OnUpdateIngameChat(int userId, Protocol.Response.InGameChat chat)
        {
            if (uuidTable.TryGetValue(userId, out var name) == false)
                return;

            _chatManager.RecvMessage(chat.Message, $"{name}");
        }

        private void OnUpdateFrame(Frame f)
        {
            Debug.Log($"InputTurn({InputTurn}) > OutputTurn({OutputTurn}) + 2");

            EnqueueHeartBeat();
            _player.upgrade.Update(f);
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
            
            waitPacket = InputTurn > OutputTurn + 2;
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

        public void EnqueueChatAction(string massage)
        {
            _ = Client.Send(new Protocol.Request.InGameChat
            {
                Turn = InputTurn,
                Frame = InputFrame,
                Message = massage

            });
        }
    }
}
