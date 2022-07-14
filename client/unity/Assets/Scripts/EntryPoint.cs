using UnityEngine;

public static class EntryPoint
{
    public static AppStateService appStateService { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        var allStates = CrashResources.LoadAppStatesFromResources();
        appStateService = new AppStateService(allStates);
        _ = appStateService.LoadEntrySceneAsync();
    }
}