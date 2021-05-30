using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UnityObjects : MonoBehaviour
    {
        [SerializeField]
        private List<Camera> _cameras;
        [SerializeField]
        private List<Light> _lights;
        
        private List<Unit> _units;

        private void Awake()
        {
            UnityResources._instance.Register(this);
        }

        private void OnDestroy()
        {
            UnityResources._instance.Unregister(this);
        }

        public Camera GetCamera()
        {
            return _cameras[0];
        }

        private static bool Intersect(FrustumPlanes frustum, Bounds bound)
        {
            return true;
        }
        
        public void IntersectUnits(Plane[] frustum, List<Unit> unitList)
        {
            foreach (var unit in _units)
            {
                if (!unit.selectable) continue;
                
                if (GeometryUtility.TestPlanesAABB(frustum, unit.bounds))
                {
                    unitList.Add(unit);
                }
            }
        }
    }
}