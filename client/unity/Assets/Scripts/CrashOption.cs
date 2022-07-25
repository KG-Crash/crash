using UnityEngine;

public class CrashOption : ScriptableObject
{
    [SerializeField] private bool _moveEntrySceneWhenStart;

    public bool moveEntrySceneWhenStart => _moveEntrySceneWhenStart;
}