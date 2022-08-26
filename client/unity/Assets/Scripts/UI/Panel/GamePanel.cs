using System.Collections;
using System.Collections.Generic;
using KG;
using UnityEngine;

namespace UI
{
    public class GamePanel : KG.UIView
    {
        [SerializeField] private ButtonSingle _upgradePanelButton;
        [SerializeField] private ButtonSingle _exitButton;
        [SerializeField] private MinimapView _minimapView;
        [SerializeField] private ChattingView _chattingView;
        [SerializeField] private AttackToggleView _attackToggleView;
    }
}