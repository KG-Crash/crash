using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class EntryPoint
{
    public static AppStateService appStateService { get; private set; }
    public static KG.UIPool uiPool { get; private set; }
    public static string uiBundleName => CrashResources.LoadAppSettings().uiBundleName;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterSceneLoad()
    {
        var option = CrashResources.LoadAppSettings();
        uiPool = new KG.UIPool(CrashResources.LoadUICanvasPrefab(), option.uiBundleName);
        appStateService = new AppStateService(uiPool);
        
        if (option.moveEntrySceneWhenStart)
            _ = appStateService.LoadEntrySceneAsync();
    }
}