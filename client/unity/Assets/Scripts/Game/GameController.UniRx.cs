using System;
using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Game
{
    public struct Frame
    {
        public int currentFrame;
        public int currentTurn;
        public Fix64 deltaTime;
    }
    
    public partial class GameController
    {
        public static IObservable<Frame> updateFrameStream => Observable.EveryUpdate().Where(_ => ready && !paused && !waitPacket)
            .Select(_ => OutputFrameChunk);
    }
}