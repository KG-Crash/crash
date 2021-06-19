using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Protocol.Response;
using Shared;
using UnityEngine;

namespace Game
{
    public class GameController : MonoBehaviour
    {
        [NonSerialized] private int _playerID;
        [NonSerialized] private uint _playerTeamID;
        [NonSerialized] private Team _allPlayerByTeam;
        [NonSerialized] private Player _player;
        [NonSerialized] private List<Unit> _selectedUnits;

        [SerializeField] private UnitTable _unitPrefabTable;
        
        private void Awake()
        {
            Handler.Instance.Bind(this);

            // 임시 코드 지워야함
            var units = new Unit[]
            {
                UnitFactory.GetNewUnit(0, 0,_unitPrefabTable),
                UnitFactory.GetNewUnit(1, 0,_unitPrefabTable),
                UnitFactory.GetNewUnit(2, 0, _unitPrefabTable)
            };

            units[0].transform.position = new Vector3(0, 0, -1.44f);
            units[1].transform.position = new Vector3(0, 0, 0);
            units[2].transform.position = new Vector3(0, 0, +1.44f);

            _playerID = 0;
            _playerTeamID = 0;
            _player = new Player();
            _player.AddUnits(units);
            _allPlayerByTeam = new Team();
            _allPlayerByTeam.players.Add(_playerTeamID, new List<Player>());
            _allPlayerByTeam.players[_playerTeamID].Add(_player);
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
            
            var teamPlayers = _allPlayerByTeam.players[_playerTeamID];
            var player = teamPlayers[_playerID];
            var units = player.units.Values.ToArray();
            var selectedUnits = new List<Unit>();

            if (rect.size != Vector2.zero)
            {
                GetFrustumPlanes(selectedCamera, rect, frustumPlanes); 
                UnityObjects.IntersectUnits(frustumPlanes, units, selectedUnits);
            }
            else
            {
                var ray = selectedCamera.ScreenPointToRay(rect.position);
                UnityObjects.IntersectUnits(ray, units, selectedUnits);
            }
            
            SelectUnits(selectedUnits);
        }

        public void SelectUnits(List<Unit> selectedUnitList)
        {
            foreach (var unit in _selectedUnits.Except(selectedUnitList))
            {
                unit.Selected(false);
            }
            foreach (var unit in selectedUnitList.Except(_selectedUnits))
            {
                unit.Selected(true);
            }

            _selectedUnits = selectedUnitList;
        }

        public Vector3Int EncodePosition(Vector3 pos)
        {
            return new Vector3Int(
                Mathf.RoundToInt(pos.x * 1000.0f), 
                Mathf.RoundToInt(pos.y * 1000.0f), 
                Mathf.RoundToInt(pos.z * 1000.0f));
        }
        public Vector3 DecodePosition(Vector3Int netPos)
        {
            return new Vector3(netPos.x / 1000.0f, netPos.y / 1000.0f, netPos.z / 1000.0f);
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
                    AppendCommand(
                        CommandFactory.CreateMoveCommand(
                            unit.unitID, EncodePosition(hitInfo.point)
                            )
                        );
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void ApplyCommand(IEnumerable<ICommand> commands)
        {
            var teamPlayers = _allPlayerByTeam.players[_playerTeamID];
            var player = teamPlayers[_playerID];
            
            foreach (var command in commands)
            {
                switch (command.type)
                {
                    case CommandType.Move:
                        var unit = player.units[command.behaiveUnitID];
                        var moveCommand = (MoveCommand) command;
                        unit.MoveTo(DecodePosition(moveCommand._position));
                        break;
                    case CommandType.AttackSingleTarget:
                        break;
                    case CommandType.AttackMultiTarget:
                        break;
                }
            }
        }

        public void AppendCommand(ICommand command)
        {
            ApplyCommand(new ICommand[1] { command });
        }

        public void AppendCommand(IEnumerable<ICommand> commands)
        {
            ApplyCommand(commands);
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
