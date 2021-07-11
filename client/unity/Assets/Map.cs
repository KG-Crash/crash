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
            var boxCollder = tile.gameObject.AddComponent<BoxCollider>();
            minimumX = Mathf.Min(minimumX, boxCollder.bounds.min.x);
            maximumX = Mathf.Max(maximumX, boxCollder.bounds.max.x);

            minimumZ = Mathf.Min(minimumZ, boxCollder.bounds.min.z);
            maximumZ = Mathf.Max(maximumZ, boxCollder.bounds.max.z);
        }

        this.transform.position = new Vector3(-minimumX, this.transform.position.y, -minimumZ);

        var width = (int)Mathf.Ceil(maximumX - minimumX);
        var height = (int)Mathf.Ceil(maximumZ - minimumZ);

        var rows = 100;
        var cols = 100;

        var ratioX = rows / (double)width;
        var ratioZ = cols / (double)height;
        var maps = new float[cols, rows];

        foreach (var tile in tiles)
        {
            var boxCollider = tile.GetComponent<BoxCollider>();
            var minX = tile.gameObject.transform.position.x + (boxCollider.bounds.min.x - boxCollider.bounds.center.x);
            var maxX = tile.gameObject.transform.position.x + (boxCollider.bounds.max.x - boxCollider.bounds.center.x);
            var minZ = tile.gameObject.transform.position.z + (boxCollider.bounds.min.z - boxCollider.bounds.center.z);
            var maxZ = tile.gameObject.transform.position.z + (boxCollider.bounds.max.z - boxCollider.bounds.center.z);

            var minOffsetX = (int)(minX * ratioX);
            var maxOffsetX = (int)(maxX * ratioX);

            var minOffsetZ = (int)(minZ * ratioZ);
            var maxOffsetZ = (int)(maxZ * ratioZ);

            for (var x = minOffsetX; x <= maxOffsetX; x++)
            {
                for (var z = minOffsetZ; z <= maxOffsetZ; z++)
                {
                    var value = Mathf.Max(boxCollider.bounds.max.y, maps[x, z]);
                    if (value > maps[x, z])
                    {
                        UnityEngine.Debug.Log($"updated {value}", tile.gameObject);
                    }

                    maps[x, z] = value;
                }
            }
        }
    }

    public void Update()
    {
        
    }
}
