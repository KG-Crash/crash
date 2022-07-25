using UnityEngine;

public class CrashAppSettings : ScriptableObject
{
    [SerializeField] private bool _moveEntrySceneWhenStart;
    [SerializeField] private string _uiBundleName;

    public bool moveEntrySceneWhenStart => _moveEntrySceneWhenStart;
    public string uiBundleName => _uiBundleName;
}