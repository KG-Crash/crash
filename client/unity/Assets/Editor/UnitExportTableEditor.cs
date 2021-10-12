using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public static class UnityObjectUtil
{
    public static T LoadAndInstantiate<T>(string path) where T : UnityEngine.Object
    {
        var sourcePrefab = AssetDatabase.LoadAssetAtPath<T>(path);
        if (sourcePrefab == null)
        {
            Debug.LogError($"{sourcePrefab} 에 에셋이 없음.");
            return null;
        }

        var newObject = UnityEngine.Object.Instantiate<T>(sourcePrefab);
        if (newObject == null)
        {
            Debug.LogError($"{sourcePrefab} 에셋을 복사하지 못함.");
            return null;
        }

        return newObject;
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        var comp = go.GetComponent<T>();
        return comp != null ? comp : go.AddComponent<T>();
    }
}

public static class MaterialUtil
{
    // https://forum.unity.com/threads/access-rendering-mode-var-on-standard-shader-via-scripting.287002/
    public static void SetOpaqueToTransparent(this Material material)
    {
        var shader = material.shader;
        if (shader.name.Contains("Standard"))
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetFloat("_Mode", 2);
            material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }
        else if (shader.name.Contains("Legacy Shaders/Diffuse"))
        {
            var transparent = "Legacy Shaders/Transparent/Diffuse";
            var transparentShader = Shader.Find(transparent);
            material.shader = transparentShader;
        }
    }
}

public static class StringUtil
{
    public static string GetCommonStartString(this string[] strings)
    {
        if (strings.Length >= 2)
        {
            var sourceStr = strings[0]; 
            var len = sourceStr.Length;

            for (int i = 1; i < strings.Length; i++)
            {
                var targetStr = strings[i];
                var targetLen = targetStr.Length;
                
                for (int j = 0; j < sourceStr.Length && j < targetStr.Length; j++)
                {
                    if (sourceStr[j] != targetStr[j])
                    {
                        targetLen = j;
                        break;
                    }
                }

                if (targetLen == targetStr.Length)
                {
                    return "";
                }
                else
                {
                    len = Mathf.Min(len, targetLen);
                }
            }
            
            return sourceStr.Substring(0, len);
        }
        else
        {
            return "";
        }
    }
    
    public static string GetControllerFormatPath(this string source, string parentPath, string parentPartialPath)
    {
        return $"{parentPath}/{source}".Replace("{PARENT_ASSET_PATH}", parentPartialPath);
    }
    public static string GetClipFormatPath(this string parentPath, string parentPartialPath)
    {
        return $"{parentPath}/{parentPartialPath}";
    }

    public static string GetPartialPath(this string source, string parentPath)
    {
        return source.Replace(parentPath, "").Replace("\\", "").Replace("/", "");
    }

    public static string GetLastPath(this string path, bool exceptDot = false)
    {
        var slashFileNameIndex = path.LastIndexOf("/", StringComparison.Ordinal);
        var nameWithExtension = path.Substring(slashFileNameIndex + 1);

        if (exceptDot)
        {
            var dotIndex = nameWithExtension.LastIndexOf(".", StringComparison.Ordinal);
            return dotIndex >= 0 ? nameWithExtension.Remove(dotIndex) : nameWithExtension;   
        }
        else
        {
            return nameWithExtension;
        }
    }
    
    private static readonly char[] digits = new char[10] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

    public static (string, string) GetLastNameAndNumber(this string path)
    {
        var lastNameIndex = path.LastIndexOf(" ");
        var firstDigitIndex = path.IndexOfAny(digits, lastNameIndex < 0 ? 0 : lastNameIndex);

        var nameStartIndex = lastNameIndex < 0 ? 0 : lastNameIndex;
        var nameLength = firstDigitIndex < 0 ? path.Length - nameStartIndex : firstDigitIndex - nameStartIndex;

        var name = path.Substring(nameStartIndex, nameLength);
        var number = path.Substring(firstDigitIndex < 0 ? path.Length : firstDigitIndex);

        return (name, number);
    }

    public static bool ContainsInsensitive(this string src, string insensitive)
    {
        return src.IndexOf(insensitive, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}

[CustomEditor(typeof(UnitExportTable))]
public class UnitExportTableEditor : Editor
{
    private static readonly char[] pathSeparators = new char[2] {'/', '\\'};
    private static readonly string deadMaterialPostfix = "_dead";
    private static readonly string fbxExt = "fbx";
    private static readonly string fbxDotExt = $".{fbxExt}";
    private static readonly string materialExt = "mat";
    private static readonly string materialDotExt = $".{materialExt}";
    private static readonly string animClipExt = "anim";
    private static readonly string animClipDotExt = $".{animClipExt}";
    private static readonly string overrideControllerExt = "overrideController";
    private static readonly string overrideControllerDotExt = $".{overrideControllerExt}";
    private static readonly string metaExt = "meta";
    private static readonly string metaDotExt = $".{metaExt}";

    private bool _exportFold;

    #region Util

    public static AnimationClip FindClip(AnimationClip[] clips, string name, string postNumber)
    {
        var splitted = name.Split('_');
        clips = Array.FindAll(clips, clip =>
        {
            foreach (var name in splitted)
                if (clip.name.ContainsInsensitive(name))
                    return true;
            return false;
        });

        if (string.IsNullOrEmpty(postNumber))
        {
            if (clips.Length > 0)
            {
                return clips[0];
            }
        }
        else
        {
            if (int.TryParse(postNumber, out var integer) && integer < clips.Length)
            {
                return clips[integer];
            }
        }

        return null;
    }

    public static int GetSameNameClipCount(AnimationClip[] clips, string name)
    {
        var maxAttackCount = 0;
        foreach (var sourceAnimClip in clips)
        {
            if (sourceAnimClip.name.ContainsInsensitive(name))
                maxAttackCount++;
        }

        return maxAttackCount;
    }

    public static string CreateDirectory(string parent, string name, bool importParent = true)
    {
        var path = !string.IsNullOrEmpty(name)? $"{parent}/{name}": parent;
        Directory.CreateDirectory(path);

        if (importParent)
            AssetDatabase.ImportAsset(parent);

        return path;
    }

    public static T CreateAssetAndLoad<T>(T serialized, string parent, string name, string extension)
        where T : UnityEngine.Object
    {
        var path = $"{parent}/{name}.{extension}";
        AssetDatabase.CreateAsset(serialized, path);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    public static void CreateFileInUnity<T>(T serialized, string parent, string name, string extension,
        bool import = true) where T : UnityEngine.Object
    {
        var path = $"{parent}/{name}.{extension}";
        AssetDatabase.CreateAsset(serialized, path);

        if (import)
            AssetDatabase.ImportAsset(path);
    }

    public static AnimatorOverrideController CreateOverrideController(AnimatorController originController,
        AnimationClip[] sourceAnimClips)
    {
        var overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = originController;

        var list = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(list);

        var sourceClipStrs = list.Select(x => x.Key.name).ToArray();
        var startCommonStr = sourceClipStrs.GetCommonStartString();
        var uniqueClipStrs = sourceClipStrs.Select(s =>
        {
            if (string.IsNullOrEmpty(startCommonStr))
            {
                return s;
            }
            else
            {
                return s.Replace(startCommonStr, "");   
            }
        }).ToArray();
        
        for (var j = 0; j < list.Count; j++)
        {
            var sourceClip = list[j].Key;
            var uniqueClipName = uniqueClipStrs[j];
            var (sourceLastName, sourceNumberString) = uniqueClipName.GetLastNameAndNumber();
            var destClip = FindClip(sourceAnimClips, sourceLastName, sourceNumberString);

            list[j] = new KeyValuePair<AnimationClip, AnimationClip>(sourceClip, destClip);
        }

        overrideController.ApplyOverrides(list);

        return overrideController;
    }

    public static string[] GetUnityFileInPath(string path)
    {
         return Directory.GetFiles(path)
            .Select(path => path.Replace("\\", "/"))
            .Where(path => !path.EndsWith(metaDotExt) && !path.ContainsInsensitive("demo"))
            .ToArray();
    }

    public static string[] GetUnityFileInPath(string path, string extension)
    {
        return Directory.GetFiles(path)
            .Select(path => path.Replace("\\", "/"))
            .Where(path => !path.EndsWith(metaDotExt) && !path.ContainsInsensitive("demo") && path.EndsWith(extension))
            .ToArray();
    }

    #endregion

    public AnimationClip[] GetAnimationClips(bool useAnimController, string sourcePath)
    {
        if (!Directory.Exists(sourcePath))
        {
            Debug.LogError($"GetAnimationClips, sourcePath={sourcePath} is not exist");
            return new AnimationClip[0];
        }
        
        if (!useAnimController)
        {
            var assetClipParentPath = sourcePath;
            var animClipOrFBXPaths = GetUnityFileInPath(assetClipParentPath);
            var animClipList = new List<AnimationClip>();

            foreach (var animClipOrFBXPath in animClipOrFBXPaths)
            {
                if (animClipOrFBXPath.EndsWith(fbxDotExt, StringComparison.OrdinalIgnoreCase))
                {
                    var allData = AssetDatabase.LoadAllAssetRepresentationsAtPath(animClipOrFBXPath);

                    for (int i = 0; i < allData.Length; i++)
                    {
                        if (allData[i] is AnimationClip clip)
                        {
                            animClipList.Add(clip);
                        }
                    }
                }
                else if (animClipOrFBXPath.EndsWith(animClipDotExt, StringComparison.OrdinalIgnoreCase))
                {
                    var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animClipOrFBXPath);
                    animClipList.Add(clip);
                }
            }

            return animClipList.ToArray();
        }
        else
        {
            var controllerPaths = GetUnityFileInPath(sourcePath);
            var controllerPath = controllerPaths.Length > 0 ? controllerPaths[0] : "";
            var sourceController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

            if (sourceController == null)
            {
                Debug.LogError($"애니메이션 컨트롤러 경로에 애니메이션 컨트롤러가 없음.({controllerPath})");
                return null;
            }

            return sourceController.animationClips;
        }
    }

    public static void SetUnitInHierarchy(GameObject instance, AnimatorOverrideController overrideController,
        int maxAttackCount, MaterialContext materialContext, GameObject hightlightPrefab)
    {
        var animator = instance.GetOrAddComponent<Animator>();
        animator.runtimeAnimatorController = overrideController;

        var unit = instance.AddComponent<Unit>();
        var highlightGO =
            (GameObject) PrefabUtility.InstantiatePrefab(hightlightPrefab);
        unit.highlighted = highlightGO;
        unit.maxAttackAnimCount = maxAttackCount;
        unit.animator = animator;
        unit.OnRefreshRenderers();

        List<Material> deadMaterials = new List<Material>();
        foreach (var renderer in unit.renderers)
        {
            var sharedMaterials = renderer.sharedMaterials;
            for (int matIdx = 0; matIdx < renderer.sharedMaterials.Length; matIdx++)
            {
                var originMaterial = sharedMaterials[matIdx];
                var originMaterialPath = AssetDatabase.GetAssetPath(originMaterial);

                var (newMaterial, newDeadMaterial) = materialContext.GetMaterial(originMaterialPath, originMaterial);

                sharedMaterials[matIdx] = newMaterial;
                deadMaterials.Add(newDeadMaterial);
            }

            renderer.sharedMaterials = sharedMaterials;
        }

        unit.deadMaterials = deadMaterials.ToArray();
    }

    public class MaterialContext
    {
        public string materialParentPath { get; set; }
        private Dictionary<string, (Material, Material)> materialDict = new Dictionary<string, (Material, Material)>();

        public (Material, Material) GetMaterial(string originMaterialPath, Material originMaterial)
        {
            if (materialDict.ContainsKey(originMaterialPath))
            {
                return materialDict[originMaterialPath];
            }
            else
            {
                var newMaterial = new Material(originMaterial);
                var lastSlashIndex =
                    originMaterialPath.LastIndexOfAny(pathSeparators);
                var originMaterialName = lastSlashIndex >= 0
                    ? originMaterialPath.Substring(lastSlashIndex + 1)
                    : originMaterialPath;
                var newMaterialPath = $"{materialParentPath}/{originMaterialName}";

                Debug.Log($"newMaterialPath:{newMaterialPath}");
                AssetDatabase.CreateAsset(newMaterial, newMaterialPath);
                newMaterial = AssetDatabase.LoadAssetAtPath<Material>(newMaterialPath);
                
                var lastDotIndex =
                    originMaterialName.LastIndexOf(materialDotExt, StringComparison.Ordinal);
                var deadMaterialName = lastDotIndex >= 0
                    ? originMaterialName.Insert(lastDotIndex, deadMaterialPostfix)
                    : $"{originMaterialName}{deadMaterialPostfix}.{materialExt}";
                var newDeadMaterialPath = $"{materialParentPath}/{deadMaterialName}";
                var newDeadMaterial = new Material(originMaterial);
                newDeadMaterial.SetOpaqueToTransparent();
                Debug.Log($"newDeadMaterialPath:{newDeadMaterialPath}");
                AssetDatabase.CreateAsset(newDeadMaterial, newDeadMaterialPath);
                newDeadMaterial = AssetDatabase.LoadAssetAtPath<Material>(newDeadMaterialPath);

                materialDict.Add(originMaterialPath, (newMaterial, newDeadMaterial));
                return (newMaterial, newDeadMaterial);
            }
        }
    }

    public class PrefabContext
    {
        public string prefabParentPath { get; set; }
        public Dictionary<string, int> unitPathToID { get; private set; }
        private UnitTable unitTable;

        public PrefabContext(UnitTable unitTable)
        {
            this.unitTable = unitTable;
            unitPathToID = unitTable.GetEnumerable().ToDictionary(x => AssetDatabase.GetAssetPath(x.Value), x => x.Key);
        }

        public void InstantiateAndSave(string sourcePrefabPath, string fileName, Action<GameObject> setUnitInHierarchy)
        {
            var newPrefabAssetPath = $"{prefabParentPath}/{fileName}";
            var newObjectInHierarchy = UnityObjectUtil.LoadAndInstantiate<GameObject>(sourcePrefabPath);
            if (newObjectInHierarchy == null)
            {
                return;
            }

            setUnitInHierarchy.Invoke(newObjectInHierarchy);
            var newUnitPrefabGO = PrefabUtility.SaveAsPrefabAsset(newObjectInHierarchy, newPrefabAssetPath);
            DestroyImmediate(newObjectInHierarchy);

            var newUnit = newUnitPrefabGO.GetComponent<Unit>();
            if (unitPathToID.ContainsKey(newPrefabAssetPath))
            {
                var id = unitPathToID[newPrefabAssetPath];
                unitTable.SetOriginUnit(id, newUnit);
            }
            else
            {
                var key = unitTable.AddNewUnit(newUnit);
                unitPathToID.Add(newPrefabAssetPath, key);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetAs = target as UnitExportTable;

        if (GUILayout.Button("Export All Units"))
        {
            var projectConfigs = targetAs._projectConfig;
            UnitTableEditor.ClearNullUnit(projectConfigs._unitTable); 
            var unitTableAssetPath = AssetDatabase.GetAssetPath(projectConfigs._unitTable);
            var destUnitPrefabPath = AssetDatabase.GetAssetPath(projectConfigs._unitPrefabDir);
            var destUnitMaterialPath = AssetDatabase.GetAssetPath(projectConfigs._unitMaterialDir);
            
            var materialContext = new MaterialContext();
            var prefabContext = new PrefabContext(projectConfigs._unitTable);

            foreach (var resConfig in targetAs._resourceConfigs)
            {
                if (resConfig._skip)
                {
                    continue;
                }

                var rootAssetPath = AssetDatabase.GetAssetPath(resConfig._rootDir);
                var childAssetPaths = Directory.GetDirectories(rootAssetPath).Select(s => s.Replace("\\", "/"));


                if (resConfig._assetDirFirst)
                {
                    foreach (var assetPath in childAssetPaths)
                    {
                        var partialAssetPath = assetPath.GetPartialPath(rootAssetPath);

                        if (!Array.TrueForAll(resConfig._excludeChildDirs,
                            exdir => String.Compare(partialAssetPath, exdir, StringComparison.Ordinal) != 0))
                        {
                            continue;
                        }

                        prefabContext.prefabParentPath = CreateDirectory(destUnitPrefabPath, partialAssetPath, true);
                        materialContext.materialParentPath =
                            CreateDirectory(destUnitMaterialPath, partialAssetPath, true);

                        // prefab
                        var assetPrefabParentPath =
                            resConfig._sourcePrefabParentSubPath.GetControllerFormatPath(assetPath, partialAssetPath);
                        var prefabPaths = GetUnityFileInPath(assetPrefabParentPath);

                        // animation, controller 
                        var animParentPath =
                            resConfig._sourceControllerSubPath.GetControllerFormatPath(assetPath, partialAssetPath);
                        var sourceAnimClips = GetAnimationClips(resConfig._useAnimController, animParentPath);
                        var overrideController =
                            CreateOverrideController(resConfig._crashSourceController, sourceAnimClips);
                        var newOverrideController = CreateAssetAndLoad(overrideController,
                            prefabContext.prefabParentPath, partialAssetPath, overrideControllerExt);
                        var maxAttackCount = GetSameNameClipCount(sourceAnimClips, "Attack");

                        for (int j = 0; j < prefabPaths.Length; j++)
                        {
                            var sourcePrefabPath = prefabPaths[j];
                            var fileName = sourcePrefabPath.GetLastPath();

                            prefabContext.InstantiateAndSave(sourcePrefabPath, fileName,
                                o =>
                                {
                                    SetUnitInHierarchy(o, overrideController, maxAttackCount, materialContext,
                                        projectConfigs._unitHightPrefab);
                                });
                        }
                    }
                }
                else
                {
                    var animParentPath =
                        childAssetPaths.FirstOrDefault(s => s.ContainsInsensitive(resConfig._sourceControllerSubPath));
                    if (string.IsNullOrEmpty(animParentPath))
                    {
                        Debug.LogError(
                            $"{resConfig._desc} 의 자식디렉토리 중 {resConfig._sourceControllerSubPath}(애니메이션루트)를 포함하는 디렉토리가 없음. ");
                        continue;
                    }

                    var prefabParentPath =
                        childAssetPaths.FirstOrDefault(s => s.ContainsInsensitive(resConfig._sourcePrefabParentSubPath));
                    if (string.IsNullOrEmpty(prefabParentPath))
                    {
                        Debug.LogError(
                            $"{resConfig._desc} 의 자식디렉토리 중 {resConfig._sourcePrefabParentSubPath}(프리팹루트)를 포함하는 디렉토리가 없음. ");
                        continue;
                    }

                    var prefabPaths =
                        Directory.GetFiles(prefabParentPath).Where(s => !s.EndsWith(".meta"))
                            .Select(s => s.Replace("\\", "/")).ToArray();
                    var commonStartPath = prefabPaths.GetCommonStartString();
                    var prefabChunks = prefabPaths.Select(s =>
                    {
                        var prefabFilePath = s.GetLastPath();
                        var prefabFileName = s.GetLastPath(true);
                        var uniquePrefabName = s;
                        if (resConfig._excludePrefixForMap)
                        {
                            uniquePrefabName = uniquePrefabName.Replace(commonStartPath, "");
                        }
                        uniquePrefabName = uniquePrefabName.GetLastPath(true);
                        
                        return (s, prefabFilePath, prefabFileName, uniquePrefabName);
                    });
                    foreach (var (prefabPath, prefabFilePath, prefabFileName, uniquePrefabName) in prefabChunks)
                    {
                        prefabContext.prefabParentPath = CreateDirectory(destUnitPrefabPath, prefabFileName, true);
                        materialContext.materialParentPath =
                            CreateDirectory(destUnitMaterialPath, resConfig._sharedMaterial? "": prefabFileName, true);

                        var prefabToAnimName = 
                            resConfig._prefabToAnim.ContainsKey(uniquePrefabName)? 
                                resConfig._prefabToAnim[uniquePrefabName]: 
                                uniquePrefabName;
                        
                        var assetControllerPath = animParentPath.GetClipFormatPath(prefabToAnimName);
                        var sourceAnimClips = GetAnimationClips(resConfig._useAnimController, assetControllerPath);
                        var overrideController =
                            CreateOverrideController(resConfig._crashSourceController, sourceAnimClips);
                        var newOverrideController = CreateAssetAndLoad(overrideController,
                            prefabContext.prefabParentPath, prefabFileName, overrideControllerExt);
                        var maxAttackCount = GetSameNameClipCount(sourceAnimClips, "Attack");
                        
                        prefabContext.InstantiateAndSave(prefabPath, prefabFilePath,
                            o =>
                            { 
                                SetUnitInHierarchy(o, overrideController, maxAttackCount, materialContext,
                                    projectConfigs._unitHightPrefab);
                            });
                    }
                }
            }
            
            EditorUtility.SetDirty(projectConfigs._unitTable);
            AssetDatabase.SaveAssets();
        }
    }
}