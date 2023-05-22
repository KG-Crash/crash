using System.Collections;
using System.Collections.Generic;
using KG;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public class GamePanel : KG.UIView
    {
        [SerializeField] private ButtonSingleTMP _upgradeOpenButton;
        [SerializeField] private ButtonSingleTMP _exitButton;
        [SerializeField] private MinimapView _minimapView;
        [SerializeField] private ChattingView _chattingView;
        [SerializeField] private AttackToggleView _attackToggleView;
        [SerializeField] private GameDragView _gameDragView;
     
        public UnityEvent upgradeOpenClick => _upgradeOpenButton.onClick;
        public UnityEvent exitClick => _exitButton.onClick;
        public UnityEvent chatSendClick => _chattingView.sendButton.onClick;
        public AttackTargetChangeEvent attackTargetChange => _attackToggleView.attackTargetChange;
        public GameDragEvent gameDragEvent => _gameDragView.gameDragEvent;
        public Vector2 minimapViewSize => _minimapView.rectTransform.rect.size;
        public ChattingView chattingView => _chattingView;

        public void Initialize(Texture minimapTex)
        {
            _minimapView.minimap.texture = minimapTex;
        }

        public void Ready(int targetCount)
        {
            _attackToggleView.Initialize(targetCount);   
        }
    }
}