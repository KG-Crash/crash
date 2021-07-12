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

        this.transform.position = new Vector3(-minimumX, this.transform.position.y, -minimumZ);

        var width = (int)Mathf.Ceil(maximumX - minimumX);
        var height = (int)Mathf.Ceil(maximumZ - minimumZ);

        var rows = 128;
        var cols = 128;

        var ratioX = rows / (double)width;
        var ratioZ = cols / (double)height;
        var maps = new float?[cols, rows];

        foreach (var tile in tiles)
        {
            var collider = tile.GetComponent<MeshCollider>();
            var minX = tile.gameObject.transform.position.x + (collider.bounds.min.x - collider.bounds.center.x);
            var maxX = tile.gameObject.transform.position.x + (collider.bounds.max.x - collider.bounds.center.x);
            var minZ = tile.gameObject.transform.position.z + (collider.bounds.min.z - collider.bounds.center.z);
            var maxZ = tile.gameObject.transform.position.z + (collider.bounds.max.z - collider.bounds.center.z);

            var minOffsetX = (int)(minX * ratioX);
            var maxOffsetX = (int)(maxX * ratioX);

            var minOffsetZ = (int)(minZ * ratioZ);
            var maxOffsetZ = (int)(maxZ * ratioZ);

            for (var x = Mathf.Max(0, minOffsetX); x <= maxOffsetX; x++)
            {
                for (var z = Mathf.Max(0, minOffsetZ); z <= maxOffsetZ; z++)
                {
                    if (maps[x, z] == null)
                        maps[x, z] = collider.bounds.max.y;
                    else
                        maps[x, z] = Mathf.Max(collider.bounds.max.y, maps[x, z].Value);
                }
            }
        }

        var blocks = new bool[rows, cols];
        var threshold = 1.0f;
        var size = new Vector2(rows / (float)width, cols / (float)height);
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var movable = (maps[col, row] != null && maps[col, row].Value < threshold);
                blocks[row, col] = !movable;
                if (!movable)
                    continue;

                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(col / size.x, threshold, row / size.y);
                cube.transform.localScale = new Vector3(width / (float)rows, 0.1f, height / (float)cols);
                cube.transform.parent = this.transform;
            }
        }
    }

    public void Update()
    {
        
    }
}
