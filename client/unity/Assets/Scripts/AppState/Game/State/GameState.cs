using System;
using System.Collections.Generic;
using Game;
using Game.Service;
using Module;
using Network;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[AutoBindAttribute(flatBuffer: true)]
public partial class GameState : AppState
{
    [SerializeField] private UnitTable _unitPrefabTable;
    [SerializeField] private ProjectileTable _projectilehPrefabTable;

    private GameSceneContext context { get; set; }
    private Transform poolOffset { get; set; } 
    private Transform focusTransform { get; set; }
    private KG.Map map { get; set; }
    private Transform[] spawnPositions { get; set; }
    private bool networkMode { get; set; }
    private ChatService chatService { get; set; }
    public ActionService actionService { get; private set; }
    
    public Dictionary<LogicalObject, IActor> unitActorMaps { get; private set; } =
        new Dictionary<LogicalObject, IActor>();

    private Player _me;
    private TeamCollection _teams;
    private ProjectileActorPool _projectileActorPool;

    private UnitActorFactory unitActorFactory;

    [InitializeMethod]
    public void Initialize()
    {
        var roomObjects = _scene.GetRootGameObjects();
        for (var i = 0; i < roomObjects.Length; i++)
        {
            var context = roomObjects[i].GetComponentInChildren<GameSceneContext>();
            if (context != null)
            {
                this.context = context;
                poolOffset = context._poolOffset;
                focusTransform = context._focusTransform;
                map = context._map;
                spawnPositions = context._spawnPositions;
                networkMode = context._networkMode;
                chatService = context._chatService;
                break;
            }
        }

        unitActorFactory = new UnitActorFactory();

        Handler.Bind(this, Dispatcher.Instance);
        ActionHandler.Bind(this);

        FPS = Shared.Const.Time.FPS;
        TPS = Shared.Const.Time.TPS;
        IsNetworkMode = networkMode;

        Application.targetFrameRate = FPS;

        InitInput();

        _projectileActorPool = new ProjectileActorPool(_projectilehPrefabTable, 15, this, poolOffset);

        // 레디에서 이름 보내야 하지 않을까?
        _ = Client.Send(new Protocol.Request.Ready { });

        context.UpdateAsObservable().Subscribe(_ => OnUpdate());
        context.LateUpdateAsObservable().Subscribe(_ => OnLateUpdate());

        InitializeProjectileHandle();
        
        Application.targetFrameRate = FPS;
        _teams = new TeamCollection(this, this);
        actionService = new ActionService(this);
    }

    [ClearMethod]
    public void Clear()
    {
        Handler.Unbind(this);
        ActionHandler.Unbind<GameState>();
        ClearInput();
    }
}


