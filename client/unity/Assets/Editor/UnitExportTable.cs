using System;
using System.Collections.Generic;
using Game;
using KG;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ResourceConfig
{
    [SerializeField] public string _desc;
    [SerializeField] public bool _skip;
    [SerializeField] public Object _rootDir;
    [SerializeField] public bool _assetDirFirst;
    [SerializeField] public bool _useAnimController;
    [SerializeField] public string[] _excludeChildDirs;
    [SerializeField] public string _sourceControllerSubPath;
    [SerializeField] public string _sourcePrefabParentSubPath;
    // [SerializeField] public bool _noneUniformPrefabDirs;
    [SerializeField] public bool _excludePrefixForMap;
    [SerializeField] public bool _sharedMaterial;
    [SerializeField] public SerializableDictionary<string, string> _prefabToAnim; 
    [SerializeField] public AnimatorController _crashSourceController;
}

[Serializable]
public class ProjectConfig
{
    public UnityEngine.Object _unitPrefabDir;
    public UnityEngine.Object _unitMaterialDir;
    public GameObject _unitHightPrefab;
    public UnitTable _unitTable;
}

[CreateAssetMenu(fileName = "UnitExportTable", menuName = "Crash/UnitExportTable", order = 0)]
public class UnitExportTable : ScriptableObject
{
    [SerializeField] public ProjectConfig _projectConfig;
    [SerializeField] public ResourceConfig[] _resourceConfigs;
}
