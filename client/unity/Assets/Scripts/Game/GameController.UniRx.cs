using System;
using System.Collections;
using System.Collections.Generic;
using FixMath.NET;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Game
{
    public partial class GameController
    {
        public static IObservable<Fix64> gameFrameStream => Observable.EveryUpdate().Where(_ => !GameController.paused).Select(_ => GameController.TimeDelta);

        private void InitializeUniRx()
        {
            var disposable = gameFrameStream.Subscribe(OnUpdateFrame);
            this.UpdateAsObservable().Subscribe(_ => OnUpdateAlways());
            this.OnDestroyAsObservable().Subscribe(_ => disposable.Dispose());
        }
    }
}