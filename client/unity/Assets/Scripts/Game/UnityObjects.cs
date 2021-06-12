using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UnityObjects : MonoBehaviour
    {
        [SerializeField] private List<Camera> _cameras;
        [SerializeField] private Transform _focus;
        [SerializeField] private List<Light> _lights;
        [SerializeField] private List<Unit> _units;
        
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

        public Transform GetFocus()
        {
            return _focus;
        }
        
        public void IntersectUnits(Plane[] frustum, List<Unit> unitList, int team)
        {                  
            foreach (var unit in _units)
            {
                if (!unit.selectable || unit.team != team) continue;
                
                if (GeometryUtility.TestPlanesAABB(frustum, unit.bounds))
                {
                    unitList.Add(unit);
                }
            }
        }
        
        public void IntersectUnits(Ray ray, List<Unit> unitList, int team)
        {                  
            Debug.Log("IntersectUnits");

            foreach (var unit in _units)
            {
                if (!unit.selectable || unit.team != team) continue;
                
                if (unit.bounds.IntersectRay(ray))
                {
                    unitList.Add(unit);
                }
            }
        }
    }
}