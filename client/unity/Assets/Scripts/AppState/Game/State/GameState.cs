using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Service;
using Module;
using Network;
using UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

[UIBind(typeof(GamePanel), true, typeof(UpgradePanel), false)]
[AutoBind(true, true)]
public partial class GameState : AppState
{
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
        var firstRoomObject = _scene.GetRootGameObjects()
            .First(x => x.TryGetComponent<GameSceneContext>(out var _));
        if (firstRoomObject != null)
        {
            context = firstRoomObject.GetComponent<GameSceneContext>();
            
            poolOffset = context._poolOffset;
            focusTransform = context._focusTransform;
            map = context._map;
            spawnPositions = context._spawnPositions;
            networkMode = context._networkMode;
            chatService = context._chatService;
            
            context.UpdateAsObservable().Subscribe(_ => OnUpdate());
            context.LateUpdateAsObservable().Subscribe(_ => OnLateUpdate());
        }

        unitActorFactory = new UnitActorFactory();
        
        FPS = Shared.Const.Time.FPS;
        TPS = Shared.Const.Time.TPS;
        IsNetworkMode = networkMode;

        Application.targetFrameRate = FPS;

        InitInput();

        _projectileActorPool = new ProjectileActorPool(ProjectileTable.Get(), 15, this, poolOffset);

        InitializeProjectileHandle();
        
        Application.targetFrameRate = FPS;
        _teams = new TeamCollection(this, this);
        actionService = new ActionService(this);

        LockStep.Reset();
        
        InitializeGamePanel();
        InitializeUpgradePanel();

        _ = Client.Send(new Protocol.Request.Ready { });
    }

    [ClearMethod]
    public void Clear()
    {
        ClearGamePanel();
        ClearUpgradePanel();
        ClearInput();
        
        unitActorMaps.Clear();

        if (context)
        {
            Destroy(context);
            context = null;  
            
            poolOffset = null;
            focusTransform = null;
            map = null;
            spawnPositions = null;
            chatService = null;
        }
    }
}
