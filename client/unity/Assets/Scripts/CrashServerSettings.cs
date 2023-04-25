using UnityEngine;

[CreateAssetMenu]
public class CrashServerSettings : ScriptableObject
{
    [SerializeField] private bool _connectToRemote;
    [SerializeField] private string _localLobbyServerAddress = "http://localhost:8080";
    [SerializeField] private string _remoteLobbyServerAddress = "http://175.196.121.225:8001";

    public string lobbyServerAddress => _connectToRemote? _remoteLobbyServerAddress: _localLobbyServerAddress;
}