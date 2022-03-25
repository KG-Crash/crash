using FixMath.NET;
using Game;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor
{
}

public static class UnitGizmoDrawer
{
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    public static void OnDrawGizmo(Unit unit, GizmoType type)
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < unit._cellPath.Count - 1; i++)
        {
            var begin = unit._cellPath[i];
            var end = unit._cellPath[i + 1];
            Gizmos.DrawLine(begin.position, end.position);
        }

        //var area = this.collisionBox;
        //Gizmos.DrawLine(new Vector3(area.minX, 0, area.minY), new Vector3(area.maxX, 0, area.minY));
        //Gizmos.DrawLine(new Vector3(area.minX, 0, area.maxY), new Vector3(area.maxX, 0, area.maxY));
        //Gizmos.DrawLine(new Vector3(area.minX, 0, area.minY), new Vector3(area.minX, 0, area.maxY));
        //Gizmos.DrawLine(new Vector3(area.maxX, 0, area.minY), new Vector3(area.maxX, 0, area.maxY));

        //Gizmos.color = Color.red;
        var cells = unit.collisionCells;
        foreach (var cell in cells)
        {
            var cellBox = new FixRect(cell.position.x, cell.position.y, cell.size, cell.size);

            Gizmos.DrawLine(new Vector3(cellBox.minX, 0, cellBox.minY), new Vector3(cellBox.maxX, 0, cellBox.minY));
            Gizmos.DrawLine(new Vector3(cellBox.minX, 0, cellBox.maxY), new Vector3(cellBox.maxX, 0, cellBox.maxY));
            Gizmos.DrawLine(new Vector3(cellBox.minX, 0, cellBox.minY), new Vector3(cellBox.minX, 0, cellBox.maxY));
            Gizmos.DrawLine(new Vector3(cellBox.maxX, 0, cellBox.minY), new Vector3(cellBox.maxX, 0, cellBox.maxY));
        }
        
        var targetStr = Unit.IsNullOrDead(unit._target) ? unit._target == null? "null": $"dead({unit._target.unitUniqueID})" : unit._target.unitUniqueID.ToString();
        var destStr = unit._cellPath.Count > 0
            ? $"{unit._cellPath.Find(x => true).center.ToString("0.##", true)} -> {unit._cellPath.FindLast(x => true).center.ToString("0.##", true)}"
            : "none";

        Handles.Label(unit.transform.position, $"{unit.unitUniqueID},{unit.hp},{unit._currentState},{targetStr},{destStr}");
    }
}