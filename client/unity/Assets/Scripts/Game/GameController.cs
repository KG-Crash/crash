using Network;
using Newtonsoft.Json;
using Protocol.Response;
using Shared.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            foreach (var pair in Table.From<TableSampleAttribute>())
            {
                UnityEngine.Debug.Log(JsonConvert.SerializeObject(pair.Value));

                foreach (var sample in Table.From<TableSample>()[pair.Key])
                {
                    UnityEngine.Debug.Log(JsonConvert.SerializeObject(sample));
                }
            }
            Handler.Instance.Bind(this);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        [ContextMenu("dd")]
        private void OnEnable()
        {
            var camera = FindObjectOfType<Camera>();
            var pointBuffer = new Vector3[4];
            camera.CalculateFrustumCorners(camera.rect, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, pointBuffer);

            foreach (var pt in pointBuffer)
            {
                Debug.Log(pt);
            }
        }

        public void GetFrustumPoints(Camera camera, Rect rectSS, Vector3[] frustumPoints)
        {
            if (frustumPoints.Length < 8)
                throw new ArgumentException("frustumPoints minimum size is 8", nameof (frustumPoints));
            
            var rectCS = new Rect(
            new Vector2(rectSS.center.x / camera.pixelWidth, rectSS.center.y / camera.pixelHeight),
                new Vector2(rectSS.width / camera.pixelWidth, rectSS.height / camera.pixelHeight)
            );
            var pointBuffer = new Vector3[4];
            camera.CalculateFrustumCorners(rectCS, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, pointBuffer);
            Array.Copy(frustumPoints, pointBuffer, 4);
            camera.CalculateFrustumCorners(rectCS, camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, pointBuffer);
            Array.Copy(frustumPoints, 4, pointBuffer, 0, 4);
        }

        public void GetFrustumPlanes(Vector3[] frustumPoints, Plane[] frustumPlanes)
        {
            if (frustumPoints.Length < 8)
                throw new ArgumentException("frustumPoints minimum size is 8", nameof (frustumPoints));
            if (frustumPlanes.Length < 6)
                throw new ArgumentException("frustumPlanes minimum size is 6", nameof (frustumPlanes));

            // frustumPoints Ordering: [0] = (Smaller,Smaller), [1] = (Smaller,Bigger), [2] = (Bigger,Bigger), [3] = (Bigger,Smaller)
            // frustumPlanes Ordering: [0] = Left, [1] = Right, [2] = Down, [3] = Up, [4] = Near, [5] = Far
            
            frustumPlanes[0] = new Plane(frustumPoints[0], frustumPoints[1], frustumPoints[4]);
            frustumPlanes[1] = new Plane(frustumPoints[2], frustumPoints[3], frustumPoints[6]);
            frustumPlanes[2] = new Plane(frustumPoints[0], frustumPoints[3], frustumPoints[4]);
            frustumPlanes[3] = new Plane(frustumPoints[3], frustumPoints[2], frustumPoints[5]);
            frustumPlanes[4] = new Plane(frustumPoints[0], frustumPoints[1], frustumPoints[2]);
            frustumPlanes[5] = new Plane(frustumPoints[4], frustumPoints[5], frustumPoints[6]);
        }

        private Vector3[] frustumPoints = new Vector3[8];
        private Plane[] frustumPlanes = new Plane[6]; 
        
        public void UpdateDragRect(Rect rect)
        {
            var uobj = UnityResources._instance.Get("objects");
            var selectedCamera = uobj.GetCamera(); 
            GetFrustumPoints(selectedCamera, rect, frustumPoints);
            GetFrustumPlanes(frustumPoints, frustumPlanes);

            var units = new List<Unit>(); 
            uobj.IntersectUnits(frustumPlanes, units);
            SelectUnits(units);
        }

        public void SelectUnits(List<Unit> selectedUnitList)
        {
        }

        [FlatBufferEvent]
        public async Task<bool> OnCreateRoom(CreateRoom response)
        {
            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnJoinRoom(JoinRoom response)
        {
            return true;
        }
    }
}
