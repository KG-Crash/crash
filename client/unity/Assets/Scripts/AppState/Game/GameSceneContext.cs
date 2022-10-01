using Game.Service;
using UnityEngine;

namespace Game
{
    public class GameSceneContext : SceneContext
    {
        [SerializeField] public Transform _poolOffset; 
        [SerializeField] public Transform _focusTransform;
        [SerializeField] public KG.Map _map;
        [SerializeField] public Transform[] _spawnPositions;
        [SerializeField] public bool _networkMode;
        [SerializeField] public ChatService _chatService;
    }
}
