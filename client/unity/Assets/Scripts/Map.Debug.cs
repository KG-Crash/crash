using System.Collections.Generic;
using UnityEngine;

namespace KG
{
    public partial class Map
    {
        private List<Region> _routes; // 테스트용

        [SerializeField] public bool drawCells = false;
        [SerializeField] public bool drawEdges = false;
        [SerializeField] public bool drawRegionEdges = false;
        [SerializeField] public bool drawPathRoute = false;

        private void OnDrawCells()
        {
            if (_cells == null)
                return;

            foreach (var node in cells)
            {
                var color = (0x00FFFFFF / (regions.Count + 1)) * (node.data.region.id + 1);

                Gizmos.color = new Color(((color & 0x00FF0000) >> 16) / (float)0xFF, ((color & 0x0000FF00) >> 8) / (float)0xFF, (color & 0x000000FF) / (float)0xFF);

                var area = new Rect(node.data.col / (float)this.scale, node.data.row / (float)this.scale, 1 / (float)scale, 1 / (float)scale);
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
                var begin = new Vector3((node.data.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (node.data.row / (float)this.scale) + (1 / (float)(this.scale * 2)));
                foreach (var edge in node.edges)
                {
                    var end = new Vector3((edge.data.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (edge.data.row / (float)this.scale) + (1 / (float)(this.scale * 2)));
                    Gizmos.DrawLine(begin, end);
                }
            }
        }

        private void OnDrawRegionEdges()
        {
            foreach (var node in regions)
            {
                var begin = node.data.centroid;
                var x1 = new Vector3((begin.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (begin.row / (float)this.scale) + (1 / (float)(this.scale * 2)));
                foreach (var edge in node.edges)
                {
                    var end = edge.data.centroid;
                    var x2 = new Vector3((end.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (end.row / (float)this.scale) + (1 / (float)(this.scale * 2)));

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

            if (drawPathRoute && _routes != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < _routes.Count - 1; i++)
                {
                    var begin = _routes[i];
                    var x1 = new Vector3((begin.centroid.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (begin.centroid.row / (float)this.scale) + (1 / (float)(this.scale * 2)));

                    var end = _routes[i + 1];
                    var x2 = new Vector3((end.centroid.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (end.centroid.row / (float)this.scale) + (1 / (float)(this.scale * 2)));

                    Gizmos.DrawLine(x1, x2);
                }
            }
        }
    }
}
