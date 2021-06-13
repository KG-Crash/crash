using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Protocol.Response;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [NonSerialized] private int _playerTeam;
        [NonSerialized] private List<int> _otherPlayerTeamList;
        [NonSerialized] private List<Unit> _allUnits;
        [NonSerialized] private List<Unit> _selectedUnits;
        
        private void Awake()
        {
            Handler.Instance.Bind(this);
            
            _playerTeam = 0;
            _otherPlayerTeamList = new List<int>();
            _allUnits = new List<Unit>();
            _selectedUnits = new List<Unit>();
        }

        private void OnDestroy()
        {
            
        }

        public static Plane GetCreatePlaneFromQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, bool reverse)
        {
            Vector3[] tris = new Vector3[4];
            tris[0] = reverse? p3: p0;
            tris[1] = reverse? p2: p1;
            tris[2] = reverse? p1: p2;
            tris[3] = reverse? p0: p3;
            Plane plane;
            GeometryUtility.TryCreatePlaneFromPolygon(tris, out plane);
            return plane;
        }

        public void GetFrustumPlanes(Camera camera, Rect rectSS, Plane[] frustumPlanes)
        {
            var frustumPoints = new Vector3[8];
            var rectCS = new Rect(
            new Vector2(rectSS.center.x / camera.pixelWidth, rectSS.center.y / camera.pixelHeight),
                new Vector2(rectSS.width / camera.pixelWidth, rectSS.height / camera.pixelHeight)
            );
            
            camera.CalculateFrustumCorners(rectCS, camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumPoints);
            Array.Copy(frustumPoints, 0, frustumPoints, 4, 4);
            camera.CalculateFrustumCorners(rectCS, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumPoints);
            for (int i = 0; i < frustumPoints.Length; i++)
                frustumPoints[i] = camera.transform.TransformPoint(frustumPoints[i]);
            
            frustumPlanes[0] = GetCreatePlaneFromQuad(frustumPoints[0], frustumPoints[1], frustumPoints[5], frustumPoints[4], false);
            frustumPlanes[1] = GetCreatePlaneFromQuad(frustumPoints[2], frustumPoints[3], frustumPoints[7], frustumPoints[6], false);
            frustumPlanes[2] = GetCreatePlaneFromQuad(frustumPoints[0], frustumPoints[3], frustumPoints[7], frustumPoints[4], true);
            frustumPlanes[3] = GetCreatePlaneFromQuad(frustumPoints[1], frustumPoints[2], frustumPoints[6], frustumPoints[5], false);
            frustumPlanes[4] = GetCreatePlaneFromQuad(frustumPoints[0], frustumPoints[1], frustumPoints[2], frustumPoints[3], true);
            frustumPlanes[5] = GetCreatePlaneFromQuad(frustumPoints[4], frustumPoints[5], frustumPoints[6], frustumPoints[7], false);
        }

        private Vector3[] frustumPoints = new Vector3[8];
        private Plane[] frustumPlanes = new Plane[6]; 
        
        public void UpdateDragRect(Rect rect)
        {
            var uobj = UnityResources._instance.Get("objects");
            var selectedCamera = uobj.GetCamera();

            var units = new List<Unit>();
            if (rect.size != Vector2.zero)
            {
                GetFrustumPlanes(selectedCamera, rect, frustumPlanes); 
                uobj.IntersectUnits(frustumPlanes, units, _playerTeam);
            }
            else
            {
                var ray = selectedCamera.ScreenPointToRay(rect.position);
                uobj.IntersectUnits(ray, units, _playerTeam);
            }
            SelectUnits(units);
        }

        public void SelectUnits(List<Unit> selectedUnitList)
        {
            foreach (var unit in _selectedUnits)
            {
                unit.Selected(false);
            }
            
            _selectedUnits.Clear();
            _selectedUnits.AddRange(selectedUnitList);

            foreach (var unit in _selectedUnits)
            {
                unit.Selected(true);
            }
        }

        public bool MoveSelectedUnitTo(Vector2 positionSS)
        {           
            var uobj = UnityResources._instance.Get("objects");
            var selectedCamera = uobj.GetCamera();
            var camRay = selectedCamera.ScreenPointToRay(positionSS);

            if (Physics.Raycast(camRay, out var hitInfo, selectedCamera.farClipPlane))
            {
                foreach (var unit in _selectedUnits)
                {
                    unit.MoveTo(hitInfo.point);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void ApplyCommand(IEnumerable<Command> commands)
        {
            
        }

        public void AppendCommand(Action<Command> commandAppend)
        {
            
        }

        [FlatBufferEvent]
        public bool OnCreateRoom(CreateRoom response)
        {
            return true;
        }

        [FlatBufferEvent]
        public bool OnJoinRoom(JoinRoom response)
        {
            return true;
        }
    }
}
