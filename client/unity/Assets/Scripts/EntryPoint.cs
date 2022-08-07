using UnityEngine;

public static class EntryPoint
{
    public static AppStateService appStateService { get; private set; }
    public static KG.UIPool uiPool { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        var option = CrashResources.LoadAppSettings();
        uiPool = new KG.UIPool(CrashResources.LoadUICanvasPrefab(), option.uiBundleName);
        appStateService = new AppStateService(CrashResources.LoadAppStates(), uiPool);
        
        if (option.moveEntrySceneWhenStart)
            _ = appStateService.LoadEntrySceneAsync();
    }
}