using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sector
{ 
    public Sectors owner { get; private set; }
    public int index { get; private set; }
    public bool isLeft => index % owner.columns == 0;
    public bool isRight => index % owner.columns == owner.columns - 1;
    public bool isTop => index < owner.columns;
    public bool isBottom => index > owner.columns * (owner.rows - 1) - 1;
    public bool isLeftTop => isLeft && isTop;
    public bool isRightTop => isRight && isTop;
    public bool isLeftBottom => isLeft && isBottom;
    public bool isRightBottom => isRight && isBottom;
    public List<Sector> nears
    {
        get
        {
            var sectors = new List<Sector> { this };
            if (isLeft == false)
                sectors.Add(owner[index - 1]);

            if (isRight == false)
                sectors.Add(owner[index + 1]);

            if (isTop == false)
                sectors.Add(owner[index - owner.columns]);

            if (isBottom == false)
                sectors.Add(owner[index + owner.columns]);

            if (isLeft == false && isTop == false)
                sectors.Add(owner[index - owner.columns - 1]);

            if (isRight == false && isTop == false)
                sectors.Add(owner[index - owner.columns + 1]);

            if (isLeft == false && isBottom == false)
                sectors.Add(owner[index + owner.columns - 1]);

            if (isRight == false && isBottom == false)
                sectors.Add(owner[index + owner.columns + 1]);

            return sectors.Where(x => x != null).ToList();
        }
    }

    public Sector(Sectors owner, int index)
    {
        this.owner = owner;
        this.index = index;
    }
}

public class Sectors : IEnumerable<Sector>
{
    private readonly List<Sector> _sectors = new List<Sector>();
    
    public Map owner { get; private set; }
    public int rows { get; private set; }
    public int columns { get; private set; }

    public Sector this[int index]
    {
        get
        {
            if (index < 0 || index > _sectors.Count - 1)
                return null;
            
            return _sectors[index];
        }
    }

    public IEnumerator<Sector> GetEnumerator() => _sectors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _sectors.GetEnumerator();

    public Sector this[FixVector3 position] => At(position);

    public Sectors(Map owner, int rows, int columns)
    {
        this.owner = owner;
        this.rows = rows;
        this.columns = columns;

        var count = this.rows * this.columns;
        for (int i = 0; i < count; i++)
            _sectors.Add(new Sector(this, i));
    }

    private int Index(FixVector3 position)
    {
        var col = (position.y / (new Fix64(owner.height) * owner.scale / new Fix64(rows)));
        var row = (position.x / (new Fix64(owner.width) * owner.scale / new Fix64(columns)));
        return columns * (int)col + (int)row;
    }

    public Sector At(FixVector3 position)
    {
        return _sectors[Index(position)];
    }
}

public class Map : MonoBehaviour
{
    public int width { get; private set; } = 512;
    public int height { get; private set; } = 384;
    public Fix64 scale { get; private set; } = (Fix64)0.1f;

    public Sectors sectors { get; private set; }

    public void Start()
    {
        sectors = new Sectors(this, 2, 2);
        var sector = sectors[new FixVector3(50, 10)];
        var nears = sector.nears;

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

        var maps = new Fix64?[height, width];

        foreach (var tile in tiles)
        {
            var collider = tile.GetComponent<MeshCollider>();
            var minX = (Fix64)(tile.gameObject.transform.position.x + (collider.bounds.min.x - collider.bounds.center.x));
            var maxX = (Fix64)(tile.gameObject.transform.position.x + (collider.bounds.max.x - collider.bounds.center.x));
            var minZ = (Fix64)(tile.gameObject.transform.position.z + (collider.bounds.min.z - collider.bounds.center.z));
            var maxZ = (Fix64)(tile.gameObject.transform.position.z + (collider.bounds.max.z - collider.bounds.center.z));

            var minOffsetX = (int)(minX / scale);
            var maxOffsetX = (int)(maxX / scale);
            var minOffsetZ = (int)(minZ / scale);
            var maxOffsetZ = (int)(maxZ / scale);

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

                    if (maps[z, x] == null)
                        maps[z, x] = (Fix64)collider.bounds.max.y;
                    else
                        maps[z, x] = (Fix64)collider.bounds.max.y > maps[z, x].Value ? (Fix64)collider.bounds.max.y : maps[z, x].Value;
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
                cube.transform.position = new FixVector3(new Fix64(col) * scale, threshold, new Fix64(row) * scale);
                cube.transform.localScale = new Vector3(width / (float)height, 0.1f, height / (float)width);
                cube.transform.parent = this.transform;
            }
        }
    }

    public void Update()
    {
        
    }
}
