using System.Collections.Generic;
using System.Linq;
using System.Text;
using FixMath.NET;
using Game;
using KG;
using UnityEngine;

public class TemporalPlaceContext
{
    public Queue<Map.Cell> _nearCellQueue = new Queue<Map.Cell>();
    public Queue<Map.Cell> _visitCellQueue = new Queue<Map.Cell>();

    public static void PlaceUnit(KG.Map map, TemporalPlaceContext ctx, Unit unit, FixVector3 centerPosition)
    {
        var pos = centerPosition;

        ctx._visitCellQueue.Clear();
        ctx._nearCellQueue.Clear();
        ctx._nearCellQueue.Enqueue(map[pos]);

        while (true)
        {
            if (ctx._nearCellQueue.Count == 0)
            {
                Debug.LogError("배치할 cell 이 없음!");
                break;
            }
        
            var nowCell = ctx._nearCellQueue.Dequeue();
            Debug.LogWarning($"{nowCell.position}");
            pos = nowCell.position;
            var rect = unit.GetCollisionBox(pos);

            var collisionCells = unit.GetCollisionCells(pos);
            var regions = collisionCells.Select(x => x.region).Distinct().ToArray();
            var units = regions.SelectMany(r => r.units).Distinct().ToArray();
            var contextUnitCollide =
                units.Any(collideUnit => collideUnit.collisionBox.Contains(rect));
            if (!contextUnitCollide)
            {
                var sb = new StringBuilder($"{nowCell.position} 통과!, {regions.Length}, {units.Length}\n");
                for (var i = 0; i < regions.Length; i++)
                    sb.AppendLine(regions[i].ToString());
                Debug.LogWarning(sb.ToString());
                break;
            }

            foreach (var near in map.NearsForSpawn(nowCell))
            {
                if (!ctx._visitCellQueue.Contains(near))
                {
                    ctx._nearCellQueue.Enqueue(near);
                    ctx._visitCellQueue.Enqueue(near);
                }
            }
        }

        unit.position = pos;
    }
}