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

[UIBind(typeof(GamePanel), true, typeof(GameDebugPanel), true, typeof(UpgradePanel), false)]
[StateBind(true, true)]
public partial class GameState : AppState
{
    private KG.Map map { get; set; }
    private Transform[] spawnPositions { get; set; }
    private bool networkMode { get; set; }
    private ChatService chatService { get; set; }
    public ActionService actionService { get; private set; }
    
    public Dictionary<LogicalObject, IActor> unitActorMaps { get; private set; } =
        new Dictionary<LogicalObject, IActor>();

    private Player me { get; set; }
    private TeamCollection teams { get; set; }
    private ProjectileActorPool projectileActorPool { get; set; }
    private UnitActorFactory unitActorFactory { get; set; }
    
    [InitializeMethod(context = typeof(GameSceneContext))]
    public void Initialize(GameSceneContext context)
    {
        networkMode = context._networkMode;
        map = context._map;
        spawnPositions = context._spawnPositions;
        chatService = context._chatService;
            
        context.UpdateAsObservable().Subscribe(_ => OnUpdate());
        context.LateUpdateAsObservable().Subscribe(_ => OnLateUpdate());
        
        FPS = Shared.Const.Time.FPS;
        TPS = Shared.Const.Time.TPS;
        Application.targetFrameRate = FPS;
        Application.targetFrameRate = FPS;

        unitActorFactory = new UnitActorFactory();
        projectileActorPool = new ProjectileActorPool(ProjectileTable.Get(), 15, this, context._poolOffset);
        teams = new TeamCollection(this, this);
        actionService = new ActionService(this);

        LockStep.Reset();
        
        BindSelfMethod();
        InitInput();
        InitializeProjectileHandle();
        InitializeGamePanel(context._map, context._mainCamera);
        InitializeDebugPanel();
        InitializeUpgradePanel();

        _ = Client.Send(new Protocol.Request.Ready { });
    }

    [ClearMethod]
    public void Clear()
    {
        map = null;
        spawnPositions = null;
        chatService = null;
        
        ClearGamePanel();
        ClearDebugPanel();
        ClearUpgradePanel();
        ClearInput();
        ClearAllBinds();
        
        unitActorMaps.Clear();
    }
}
