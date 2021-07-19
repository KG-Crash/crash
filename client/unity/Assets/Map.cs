using FixMath.NET;
using System;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public void Start()
    {
        var tiles = GetComponentsInChildren<Transform>().Except(new[] { this.GetComponent<Transform>() });
        var minimumX = float.MaxValue;
        var maximumX = float.MinValue;
        var minimumZ = float.MaxValue;
        var maximumZ = float.MinValue;
        foreach (var tile in tiles)
        {
            var collider = tile.gameObject.AddComponent<MeshCollider>();
            minimumX = Mathf.Min(minimumX, collider.bounds.min.x);
            maximumX = Mathf.Max(maximumX, collider.bounds.max.x);

            minimumZ = Mathf.Min(minimumZ, collider.bounds.min.z);
            maximumZ = Mathf.Max(maximumZ, collider.bounds.max.z);
        }

        var width = 512;
        var height = 384;
        var tileWidth = (Fix64)0.1;
        var tileHeight = (Fix64)0.1;
        var maps = new Fix64?[height, width];

        foreach (var tile in tiles)
        {
            var collider = tile.GetComponent<MeshCollider>();
            var minX = (Fix64)(tile.gameObject.transform.position.x + (collider.bounds.min.x - collider.bounds.center.x));
            var maxX = (Fix64)(tile.gameObject.transform.position.x + (collider.bounds.max.x - collider.bounds.center.x));
            var minZ = (Fix64)(tile.gameObject.transform.position.z + (collider.bounds.min.z - collider.bounds.center.z));
            var maxZ = (Fix64)(tile.gameObject.transform.position.z + (collider.bounds.max.z - collider.bounds.center.z));

            var minOffsetX = (int)(minX / tileWidth);
            var maxOffsetX = (int)(maxX / tileWidth);
            var minOffsetZ = (int)(minZ / tileHeight);
            var maxOffsetZ = (int)(maxZ / tileHeight);

            for (var x = minOffsetX; x <= maxOffsetX; x++)
            {
                if (minOffsetX <= 0)
                    continue;

                if (minOffsetX >= width)
                    continue;

                for (var z = minOffsetZ; z <= maxOffsetZ; z++)
                {
                    if (minOffsetZ <= 0)
                        continue;

                    if (maxOffsetZ >= height)
                        continue;

                    try
                    {
                        if (maps[z, x] == null)
                            maps[z, x] = (Fix64)collider.bounds.max.y;
                        else
                            maps[z, x] = (Fix64)collider.bounds.max.y > maps[z, x].Value ? (Fix64)collider.bounds.max.y : maps[z, x].Value;
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log(e.Message);
                    }
                }
            }
        }

        var blocks = new bool[height, width];
        var threshold = (Fix64)1.0f;
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                var movable = (maps[row, col] != null && maps[row, col].Value < threshold);
                blocks[row, col] = !movable;
                if (!movable)
                    continue;

                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new FixVector3(new Fix64(col) * tileWidth, threshold, new Fix64(row) * tileHeight);
                cube.transform.localScale = new Vector3(width / (float)height, 0.1f, height / (float)width);
                cube.transform.parent = this.transform;
            }
        }
    }

    public void Update()
    {
        
    }
}
