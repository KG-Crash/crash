using KG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KG.Map))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        var map = (KG.Map) target;

        if (map._walkability.Length != map.cols * map.rows)
        {
            EditorGUILayout.HelpBox(
                $"워커빌리티와({map._walkability.Length}) cols*rows({map.cols * map.rows}) 숫자가 안맞음. Update Walkabilty 해줘잉",
                MessageType.Warning);
        }

        if (GUILayout.Button("워커빌리티 업데이트"))
        {
            map.OnUpdateWalkability();
        }
    }
}

public static class MapGizmoDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    public static void OnDrawGizmo(Map map, GizmoType type)
    {
        var settings = CrashSettings.GetOrCreateSettings();

        if (settings._mapDrawCells)
        {
            OnDrawCells(map);   
        }

        if (settings._mapDrawEdges)
        {
            OnDrawEdges(map);
        }

        if (settings._mapDrawRegionEdges)
        {
            OnDrawRegionEdges(map);
        }
    }

    private static void OnDrawCells(Map map)
    {
        var size = 1 / (float) map.scale;
        foreach (var node in map.cells)
        {
            if (node.data.walkable == false)
                continue;

            var color = (0x00FFFFFF / (map.regions.Count + 1)) * (node.data.region.id + 1);

            Gizmos.color = new Color(((color & 0x00FF0000) >> 16) / (float) 0xFF,
                ((color & 0x0000FF00) >> 8) / (float) 0xFF, (color & 0x000000FF) / (float) 0xFF);

            var area = new Rect(node.data.position.x, node.data.position.y, size, size);
            Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMin), new Vector3(area.xMax, 0, area.yMin));
            Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMax), new Vector3(area.xMax, 0, area.yMax));

            Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMin), new Vector3(area.xMin, 0, area.yMax));
            Gizmos.DrawLine(new Vector3(area.xMax, 0, area.yMin), new Vector3(area.xMax, 0, area.yMax));
        }
    }

    private static void OnDrawEdges(Map map)
    {
        Gizmos.color = Color.red;
        foreach (var node in map.cells)
        {
            var begin = new Vector3(node.data.center.x, 0, node.data.center.y);
            foreach (var edge in node.edges)
            {
                var end = new Vector3(edge.data.center.x, 0, edge.data.center.y);
                Gizmos.DrawLine(begin, end);
            }
        }
    }

    private static void OnDrawRegionEdges(Map map)
    {
        foreach (var node in map.regions)
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
}