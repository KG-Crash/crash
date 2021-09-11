using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KG
{
    public partial class Map
    {
        [SerializeField] public bool drawCells = false;
        [SerializeField] public bool drawEdges = false;
        [SerializeField] public bool drawRegionEdges = false;

        private void OnDrawCells()
        {
            if (_cells == null)
                return;

            var size = 1 / (float)scale;
            foreach (var node in cells)
            {
                var color = (0x00FFFFFF / (regions.Count + 1)) * (node.data.region.id + 1);

                Gizmos.color = new Color(((color & 0x00FF0000) >> 16) / (float)0xFF, ((color & 0x0000FF00) >> 8) / (float)0xFF, (color & 0x000000FF) / (float)0xFF);

                var area = new Rect(node.data.position.x, node.data.position.y, size, size);
                Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMin), new Vector3(area.xMax, 0, area.yMin));
                Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMax), new Vector3(area.xMax, 0, area.yMax));

                Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMin), new Vector3(area.xMin, 0, area.yMax));
                Gizmos.DrawLine(new Vector3(area.xMax, 0, area.yMin), new Vector3(area.xMax, 0, area.yMax));
            }
        }

        private void OnDrawEdges()
        {
            if (_cells == null)
                return;

            Gizmos.color = Color.red;
            foreach (var node in cells)
            {
                var begin = new Vector3(node.data.center.x, 0, node.data.center.y);
                foreach (var edge in node.edges)
                {
                    var end = new Vector3(edge.data.center.x, 0, edge.data.center.y);
                    Gizmos.DrawLine(begin, end);
                }
            }
        }

        private void OnDrawRegionEdges()
        {
            foreach (var node in regions)
            {
                var begin = node.data.centroid;
                var x1 = new Vector3(begin.center.x, 0, begin.center.y);
                foreach (var edge in node.edges)
                {
                    var end = edge.data.centroid;
                    var x2 = new Vector3(end.center.x, 0, end.center.y);

                    Gizmos.color = (node.data.walkable && edge.data.walkable) ? Color.green : Color.red;
                    Gizmos.DrawLine(x1, x2);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (drawCells)
                OnDrawCells();

            if (drawEdges)
                OnDrawEdges();

            if (drawRegionEdges)
                OnDrawRegionEdges();
        }
    }
}
