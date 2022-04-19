using FixMath.NET;
using UnityEngine;

namespace Game
{
    public interface IActor
    {
        FixVector3 position { set; }
        GameObject gameObject { get; }
        void LookAt(FixVector3 worldPosition);
        Transform parent { get; }
        void SetParent(Transform parent);
    }

}
