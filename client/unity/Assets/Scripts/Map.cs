using FixMath.NET;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class Sector : IEnumerable<Unit>
//{ 
//    public Sectors owner { get; private set; }
//    public int index { get; private set; }
//    public bool isLeft => index % owner.columns == 0;
//    public bool isRight => index % owner.columns == owner.columns - 1;
//    public bool isTop => index < owner.columns;
//    public bool isBottom => index > owner.columns * (owner.rows - 1) - 1;
//    public bool isLeftTop => isLeft && isTop;
//    public bool isRightTop => isRight && isTop;
//    public bool isLeftBottom => isLeft && isBottom;
//    public bool isRightBottom => isRight && isBottom;
//    public List<Sector> nears
//    {
//        get
//        {
//            var sectors = new List<Sector> { this };
//            if (isLeft == false)
//                sectors.Add(owner[index - 1]);

//            if (isRight == false)
//                sectors.Add(owner[index + 1]);

//            if (isTop == false)
//                sectors.Add(owner[index - owner.columns]);

//            if (isBottom == false)
//                sectors.Add(owner[index + owner.columns]);

//            if (isLeft == false && isTop == false)
//                sectors.Add(owner[index - owner.columns - 1]);

//            if (isRight == false && isTop == false)
//                sectors.Add(owner[index - owner.columns + 1]);

//            if (isLeft == false && isBottom == false)
//                sectors.Add(owner[index + owner.columns - 1]);

//            if (isRight == false && isBottom == false)
//                sectors.Add(owner[index + owner.columns + 1]);

//            return sectors.Where(x => x != null).ToList();
//        }
//    }
//    public List<Unit> units { get; private set; } = new List<Unit>();

//    public Sector(Sectors owner, int index)
//    {
//        this.owner = owner;
//        this.index = index;
//    }

//    public IEnumerator<Unit> GetEnumerator() => units.GetEnumerator();

//    IEnumerator IEnumerable.GetEnumerator() => units.GetEnumerator();
//}

//public class Sectors : IEnumerable<Sector>
//{
//    private readonly List<Sector> _sectors = new List<Sector>();

//    public Map owner { get; private set; }
//    public int rows { get; private set; }
//    public int columns { get; private set; }

//    public Sector this[int index] => At(index);

//    public IEnumerator<Sector> GetEnumerator() => _sectors.GetEnumerator();

//    IEnumerator IEnumerable.GetEnumerator() => _sectors.GetEnumerator();

//    public Sector this[FixVector3 position] => At(position);

//    public Sectors(Map owner, int rows, int columns)
//    {
//        this.owner = owner;
//        this.rows = rows;
//        this.columns = columns;

//        var count = this.rows * this.columns;
//        for (int i = 0; i < count; i++)
//            _sectors.Add(new Sector(this, i));
//    }

//    private int Index(FixVector3 position)
//    {
//        var col = (position.y / (new Fix64(owner.height) * owner.scale / new Fix64(rows)));
//        var row = (position.x / (new Fix64(owner.width) * owner.scale / new Fix64(columns)));
//        return columns * (int)col + (int)row;
//    }

//    public Sector At(int index)
//    {
//        if (index < 0 || index > _sectors.Count - 1)
//            return null;

//        return _sectors[index];
//    }

//    public Sector At(FixVector3 position)
//    {
//        var index = Index(position);
//        return At(index);
//    }
//}

public class WalkabilityTable
{
    private readonly List<(Vector3, Vector3)> _points = new List<(Vector3, Vector3)>();
    private readonly bool[,] _walkability;

    public int cols { get; private set; }
    public int rows { get; private set; }
    public int scale { get; private set; }

    public static implicit operator bool[,](WalkabilityTable walkabilityTable) => walkabilityTable._walkability;

    public bool this[FixVector2 position]
    {
        get => _walkability[ToIndex(position.y), ToIndex(position.x)];
        set => _walkability[ToIndex(position.y), ToIndex(position.x)] = value;
    }

    public bool this[int row, int col]
    {
        get => _walkability[row, col];
        set => _walkability[row, col] = value;
    }

    public WalkabilityTable(int width, int height, int scale)
    {
        this.cols = width * scale;
        this.rows = height * scale;
        this.scale = scale;

        for (int i = 0; i < height * scale; i++)
        {
            var pivot = i / (float)scale;
            var points = (new Vector3(0, 0, pivot), new Vector3(width, 0, pivot));
            this._points.Add(points);
        }

        for (int i = 0; i < width * scale; i++)
        {
            var pivot = i / (float)scale;
            var points = (new Vector3(pivot, 0, 0), new Vector3(pivot, 0, height));
            this._points.Add(points);
        }

        _walkability = new bool[height * scale, width * scale];
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var (begin, end) in _points)
        {
            Gizmos.DrawLine(begin, end);
        }


        Gizmos.color = Color.green;
        for (int row = 0; row < this.rows; row++)
        {
            for (int col = 0; col < this.cols; col++)
            {
                var walkability = this[row, col];
                if (walkability == false)
                    continue;

                var area = new Rect(col / (float)this.scale, row / (float)this.scale, 1 / (float)scale, 1 / (float)scale);
                Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMin), new Vector3(area.xMax, 0, area.yMin));
                Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMax), new Vector3(area.xMax, 0, area.yMax));

                Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMin), new Vector3(area.xMin, 0, area.yMax));
                Gizmos.DrawLine(new Vector3(area.xMax, 0, area.yMin), new Vector3(area.xMax, 0, area.yMax));
            }
        }
    }

    public int ToIndex(Fix64 value)
    {
        return (int)((float)value * this.scale);
    }
}

public class Map : MonoBehaviour
{
    public int width { get; private set; } = 192;
    public int height { get; private set; } = 168;
    public int scale { get; private set; } = 4;
    //public Sectors sectors { get; private set; }
    public WalkabilityTable walkabilityTable { get; private set; }

    private void OnDrawGizmos()
    {
        walkabilityTable?.OnDrawGizmos();
    }

    public void Start()
    {
        walkabilityTable = new WalkabilityTable(width, height, scale);

        //sectors = new Sectors(this, 2, 2);
        //var sector = sectors[new FixVector3(50, 10)];
        //var nears = sector?.nears;

        var tiles = GetComponentsInChildren<Transform>().Except(new[] { this.GetComponent<Transform>() });
        foreach (var tile in tiles)
        {
            tile.gameObject.AddComponent<MeshCollider>();
        }

        var threshold = 1.0f;
        var half = new Vector3(1 / (float)(scale * 2), 0.1f, 1 / (float)(scale * 2));
        foreach (var collider in tiles.Select(x => x.gameObject.GetComponent<MeshCollider>()))
        {
            var min = collider.bounds.min;
            var max = collider.bounds.max;

            var begin = (col: walkabilityTable.ToIndex(min.x), row: walkabilityTable.ToIndex(min.z));
            var end = (col: walkabilityTable.ToIndex(max.x), row: walkabilityTable.ToIndex(max.z));
            for (int row = begin.row; row <= end.row; row++)
            {
                for (int col = begin.col; col <= end.col; col++)
                {
                    var center = new Vector3(col / (float)scale + half.x, threshold, row / (float)scale + half.z);
                    var hits = Physics.BoxCastAll(center, half, Vector3.down).Where(x => x.collider == collider).ToArray();
                    if (hits.Length == 0)
                        continue;

                    var hit = hits.FirstOrDefault();
                    if (hit.point.y > threshold)
                        continue;

                    walkabilityTable[row, col] = true;
                }
            }
        }
    }

    public void Update()
    {
        
    }
}


public class AStar
{
    #region Node
    private class Node
    {
        public FixVector2 position { get; private set; }
        public bool walkable { get; private set; }
        public int G { get; set; } = 0;
        public int H { get; set; } = 0;
        public int F => G + H;
        public Node parent { get; set; } = null;

        public Node(FixVector2 position, bool walkable)
        {
            this.position = position;
            this.walkable = walkable;
        }
    }
    #endregion

    private readonly Node[,] _nodes; // [y,x]
    public int rows { get; private set; }
    public int cols { get; private set; }

    private AStar(bool[,] maps)
    {
        rows = maps.GetLength(0);
        cols = maps.GetLength(1);
        
        _nodes = new Node[rows, cols];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                _nodes[row, col] = new Node(new FixVector2(new Fix64(col), new Fix64(row)), maps[row, col]);
            }
        }
    }

    /// <summary>
    /// 인접노드 가져옴
    /// </summary>
    /// <param name="from">시작점</param>
    /// <returns></returns>
    private List<Node> Nears(Node from)
    {
        var nears = new List<Node>();

        var isLeft = (from.position.x == Fix64.Zero);
        if (!isLeft)
            nears.Add(_nodes[from.position.y, from.position.x - 1]);

        var isTop = (from.position.y == 0);
        if (!isTop)
            nears.Add(_nodes[from.position.y - 1, from.position.x]);

        var isRight = (from.position.x == cols - 1);
        if (!isRight)
            nears.Add(_nodes[from.position.y, from.position.x + 1]);

        var isBottom = (from.position.y == rows - 1);
        if (!isBottom)
            nears.Add(_nodes[from.position.y + 1, from.position.x]);

        if (!isLeft && !isTop)
            nears.Add(_nodes[from.position.y - 1, from.position.x - 1]);

        if (!isRight && !isTop)
            nears.Add(_nodes[from.position.y - 1, from.position.x + 1]);

        if (!isLeft && !isBottom)
            nears.Add(_nodes[from.position.y + 1, from.position.x - 1]);

        if (!isRight && !isBottom)
            nears.Add(_nodes[from.position.y + 1, from.position.x + 1]);

        return nears.Where(x => x.walkable).ToList();
    }

    /// <summary>
    /// 노드 기반으로 경로탐색
    /// </summary>
    /// <param name="begin">시작</param>
    /// <param name="end">종료</param>
    /// <returns></returns>
    private Stack<Node> Find(Node begin, Node end)
    {
        var result = new Stack<Node>();
        var openedList = new List<Node>();
        var closedList = new List<Node>();

        openedList.Add(begin);
        while (openedList.Count > 0 && closedList.Contains(end) == false)
        {
            var current = openedList.First();
            openedList.Remove(current);
            closedList.Add(current);

            var nears = Nears(current)
                .Where(x => closedList.Contains(x) == false)
                .Where(x => openedList.Contains(x) == false)
                .ToList();

            foreach (var near in nears)
            {
                near.parent = current;
                near.G = (near.position - current.position).sqrMagnitude;
                near.H = (near.position - end.position).sqrMagnitude;
            }

            openedList.AddRange(nears);
            openedList.Sort((x, y) => x.F.CompareTo(y.F)); // OrderBy보다 빠른듯
        }

        if (closedList.Contains(end) == false)
            return result;

        var tracked = end;
        while (tracked != begin)
        {
            result.Push(tracked);
            tracked = tracked.parent;
        }
        return result;
    }

    /// <summary>
    /// 경로 탐색
    /// </summary>
    /// <param name="maps">맵</param>
    /// <param name="begin">시작점</param>
    /// <param name="end">종료점</param>
    /// <returns></returns>
    public static List<FixVector2> Find(bool[,] maps, FixVector2 begin, FixVector2 end)
    {
        var astar = new AStar(maps);
        var root = astar.Find(astar._nodes[begin.y, begin.x], astar._nodes[end.y, end.x]);
        return root.Select(x => x.position).ToList();
    }
}