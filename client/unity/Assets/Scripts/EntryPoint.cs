using UnityEngine;

public static class EntryPoint
{
    public static AppStateService appStateService { get; private set; }
    public static UIPool uiPool { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        uiPool = new UIPool(CrashResources.LoadUICanvasPrefab());
        appStateService = new AppStateService(CrashResources.LoadAppStates(), uiPool);
        _ = appStateService.LoadEntrySceneAsync();
    }
}