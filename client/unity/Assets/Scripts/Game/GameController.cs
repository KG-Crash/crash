using FixMath.NET;
using Game.Service;
using Module;
using Network;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public static class ActionExtension
    {
        public static ushort LOWORD(this uint actionParam) => (ushort)(actionParam & 0x0000FFFF);
        public static ushort HIWORD(this uint actionParam) => (ushort)((actionParam & 0xFFFF0000) >> 16);

        public static FixVector2 WORD2POS(this uint actionParam) => new FixVector2(Fix64.One * actionParam.LOWORD(), Fix64.One * actionParam.HIWORD());
        
        public static uint TOWORD(ushort hiword, ushort loword)
        {
            return loword | (uint)hiword << 16;
        }

        public static uint TOWORD(FixVector2 v2)
        {
            return (uint)v2.x | (uint)v2.y << 16;
        }
        
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

    public static class LockStep
    {
        public class Pair
        {
            public int In { get; set; }
            public int Out { get; set; }
        }

        public static Pair Frame { get; private set; } = new Pair();
        public static Pair Turn { get; private set; } = new Pair();
    }

    public partial class GameController : MonoBehaviour, ActionService.Listener
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
        
        public static int InputTotalFrame => LockStep.Frame.In + LockStep.Turn.In * Shared.Const.Time.FramePerTurn;

        public static Frame InputFrameChunk => new Frame()
            {currentFrame = LockStep.Frame.In, currentTurn = LockStep.Turn.In, deltaTime = TimeDelta};
        
        public static int OutputTotalFrame => LockStep.Frame.Out + LockStep.Turn.Out * Shared.Const.Time.FramePerTurn;

        public static Frame OutputFrameChunk => new Frame()
            {currentFrame = LockStep.Frame.Out, currentTurn = LockStep.Turn.Out, deltaTime = TimeDelta};


        public ActionService ActionService { get; private set; }


        public Dictionary<LogicalObject, IActor> unitActorMaps { get; private set; } = new Dictionary<LogicalObject, IActor>();

        [NonSerialized] private Player _me;
        [NonSerialized] private TeamCollection _teams;
        [NonSerialized] private ProjectileActorPool _projectileActorPool;
        [NonSerialized] private ChatService _chatManager;

        [Header("Miscellaneous")] 
        [SerializeField] private Transform _poolOffset; 
        [SerializeField] private Transform _focusTransform;
        
        [SerializeField] private KG.Map _map;
        [FormerlySerializedAs("_unitFactory")] [SerializeField] private UnitActorFactory unitActorFactory;
        [SerializeField] private UnitTable _unitPrefabTable;
        [SerializeField] private ProjectileTable _projectilehPrefabTable;
        [SerializeField] private bool _networkMode;
        [SerializeField] private Transform[] _spawnPositions;
        
        private void Awake()
        {
            Handler.Bind(this, Dispatcher.Instance);
            ActionHandler.Bind(this);

            FPS = Shared.Const.Time.FPS;
            TPS = Shared.Const.Time.TPS;
            IsNetworkMode = _networkMode;

            Application.targetFrameRate = FPS;
            
            InitInput();
            
            _projectileActorPool = new ProjectileActorPool(_projectilehPrefabTable, 15, this, _poolOffset);
            _chatManager = this.gameObject.GetComponent<ChatService>();

            // 레디에서 이름 보내야 하지 않을까?
            _ = Client.Send(new Protocol.Request.Ready{ });
            
            InitializeUniRx();
            InitializeProjectileHandle();
        }

        private void Start()
        {
            Application.targetFrameRate = FPS;
            _teams = new TeamCollection(this, this);
            ActionService = new ActionService(this);
        }

        private void OnUpdate()
        {
            OnUpdateAlwaysDebug();

            if (!ready || paused)
            {
                return;
            }
            
            waitPacket = LockStep.Turn.In > LockStep.Turn.Out + 2;

            if(ActionService.Update())
                LockStep.Turn.Out++;
        }

        public void OnAction(int userId, Protocol.Response.Action action)
        {
            ActionHandler.Execute<GameController>(userId, action);
        }

        public void OnChat(int userId, Protocol.Response.InGameChat chat)
        {
            if (uuidTable.TryGetValue(userId, out var name) == false)
                return;

            _chatManager.RecvMessage(chat.Message, $"{name}");
        }

        private void OnUpdateFrame(Frame f)
        {
            EnqueueHeartBeat();
            _me.upgrade.Update(f);
            OnUpdateFrameDebug(f);
        }

        private void OnLateUpdateFrame(Frame f)
        {
            Debug.Log($"LockStep.Turn.In({LockStep.Turn.In}) > LockStep.Turn.Out({LockStep.Turn.Out}) + 2");
            
            if (++LockStep.Frame.In >= Shared.Const.Time.FramePerTurn)
            {
                if (IsNetworkMode)
                {
                    ActionService.Flush();
                    Debug.Log($"send queue : {LockStep.Turn.In}");
                }

                LockStep.Turn.In++;
                LockStep.Frame.In = 0;
            }
            
            waitPacket = LockStep.Turn.In > LockStep.Turn.Out + 2;
        }

        private void OnDestroy()
        {
            ClearInput();
        }
    }
}
