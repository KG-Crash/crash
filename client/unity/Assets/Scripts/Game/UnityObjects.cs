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
        
        public static void IntersectUnits(Plane[] frustum, IEnumerable<Unit> sourceUnits, List<Unit> unitList)
        {                  
            foreach (var unit in sourceUnits)
            {
                if (!unit.selectable) continue;
                
                if (GeometryUtility.TestPlanesAABB(frustum, unit.bounds))
                {
                    unitList.Add(unit);
                }
            }
        }
        
        public static void IntersectUnits(Ray ray, IEnumerable<Unit> sourceUnits, List<Unit> outUnitList)
        {                  
            Debug.Log("IntersectUnits");

            foreach (var unit in sourceUnits)
            {
                if (!unit.selectable) continue;
                
                if (unit.bounds.IntersectRay(ray))
                {
                    outUnitList.Add(unit);
                }
            }
        }
    }
}