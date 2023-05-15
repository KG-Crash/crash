using UnityEngine;

public class AppStateSettings : ScriptableObject
{
    [SerializeField] private string _sceneName;
    [SerializeField] private bool _entryScene;
    
    public string sceneName => _sceneName;
    public bool entryScene => _entryScene;
}