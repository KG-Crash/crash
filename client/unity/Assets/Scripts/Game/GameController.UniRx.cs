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
        public static IObservable<Frame> updateFrameStream => Observable.EveryUpdate().Where(_ => !paused)
            .Select(_ => InputFrameChunk);

        public static IObservable<Frame> lateUpdateFrameStream => Observable.EveryLateUpdate().Where(_ => !paused)
            .Select(_ => InputFrameChunk);

        private void InitializeUniRx()
        {
            var updateDisposable = updateFrameStream.Subscribe(OnUpdateFrame);
            var lateUpdateDisposable = lateUpdateFrameStream.Subscribe(OnLateUpdateFrame);
            
            this.UpdateAsObservable().Subscribe(_ => OnUpdateAlways());
            this.OnDestroyAsObservable().Subscribe(_ =>
            {
                updateDisposable.Dispose();
                lateUpdateDisposable.Dispose();
            });
        }
    }
}