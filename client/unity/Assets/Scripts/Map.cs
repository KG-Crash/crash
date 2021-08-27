using FixMath.NET;
using System;
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

public static class CellExtension
{
    public static CellCollection.Cell Find(this CellCollection collection, Func<CellCollection.Cell, bool> predicate)
    {
        for (int row = 0; row < collection._cells.GetLength(0); row++)
        {
            for (int col = 0; col < collection._cells.GetLength(1); col++)
            {
                var cell = collection[row, col];
                if (predicate(cell))
                    return cell;
            }
        }

        return null;
    }

    public static (int row, int col) Centroid(this List<CellCollection.Cell> cells)
    {
        ulong accumulatedArea = 0;
        var centerX = 0;
        var centerY = 0;

        for (int i = 0, j = cells.Count - 1; i < cells.Count; j = i++)
        {
            var temp = cells[i].col * cells[j].row - cells[j].col * cells[i].row;
            accumulatedArea += (ulong)temp;
            centerX += (cells[i].col + cells[j].col) * temp;
            centerY += (cells[i].row + cells[j].row) * temp;
        }

        accumulatedArea *= 3;
        return ((int)(centerX / (float)accumulatedArea), (int)(centerY / (float)accumulatedArea));
    }
}

public class CellCollection
{
    public class Region
    { 
        public int id { get; set; }
        public bool walkable { get; set; }
        public Cell centroid { get; set; }
    }

    public class Cell
    {
        public int row { get; set; }
        public int col { get; set; }
        public Region? region { get; set; }
        public bool walkable { get; set; }
    }

    public Cell[] _cells { get; private set; }
    public Graph<Cell> cells { get; private set; } = new Graph<Cell>();
    public Graph<Region> regions { get; private set; } = new Graph<Region>();
    public int cols { get; private set; }
    public int rows { get; private set; }
    public int scale { get; private set; }

    public static implicit operator Cell[](CellCollection walkabilityTable) => walkabilityTable._cells;

    public Cell this[FixVector2 position]
    {
        get => this[ToIndex(position.y), ToIndex(position.x)];
        set => this[ToIndex(position.y), ToIndex(position.x)] = value;
    }

    public Cell this[int row, int col]
    {
        get => _cells[row * cols + col];
        set => _cells[row * cols + col] = value;
    }

    public CellCollection(int width, int height, int scale)
    {
        this.cols = width * scale;
        this.rows = height * scale;
        this.scale = scale;

        var rows = height * scale;
        var cols = width * scale;
        _cells = new Cell[rows * cols];
        for (int i = 0; i < rows * cols; i++)
        {
            var row = i / cols;
            var col = i % cols;

            _cells[cols * row + col] = new Cell
            {
                row = row,
                col = col,
                walkable = false,
                region = null
            };
        }
    }

    public void OnDrawCells()
    {
        foreach (var cell in _cells)
        {
            var color = (0x00FFFFFF / (regions.nodes.Count + 1)) * (cell.region.id + 1);

            Gizmos.color = new Color(((color & 0x00FF0000) >> 16) / (float)0xFF, ((color & 0x0000FF00) >> 8) / (float)0xFF, (color & 0x000000FF) / (float)0xFF);

            var area = new Rect(cell.col / (float)this.scale, cell.row / (float)this.scale, 1 / (float)scale, 1 / (float)scale);
            Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMin), new Vector3(area.xMax, 0, area.yMin));
            Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMax), new Vector3(area.xMax, 0, area.yMax));

            Gizmos.DrawLine(new Vector3(area.xMin, 0, area.yMin), new Vector3(area.xMin, 0, area.yMax));
            Gizmos.DrawLine(new Vector3(area.xMax, 0, area.yMin), new Vector3(area.xMax, 0, area.yMax));
        }
    }

    public void OnDrawEdges()
    {
        Gizmos.color = Color.red;
        foreach (var cell in _cells)
        {
            var begin = new Vector3((cell.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (cell.row / (float)this.scale) + (1 / (float)(this.scale * 2)));
            foreach (var edge in cells.nodes[cell].edges)
            {
                var end = new Vector3((edge.data.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (edge.data.row / (float)this.scale) + (1 / (float)(this.scale * 2)));
                Gizmos.DrawLine(begin, end);
            }
        }
    }

    public void OnDrawRegionEdges()
    {
        foreach (var node in regions.nodes)
        {
            var begin = node.data.centroid;
            var begin1 = new Vector3((begin.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (begin.row / (float)this.scale) + (1 / (float)(this.scale * 2)));
            foreach (var edge in node.edges)
            {
                var end = edge.data.centroid;
                var end1 = new Vector3((end.col / (float)this.scale) + (1 / (float)(this.scale * 2)), 0, (end.row / (float)this.scale) + (1 / (float)(this.scale * 2)));

                Gizmos.color = (node.data.walkable && edge.data.walkable) ? Color.green : Color.red;
                Gizmos.DrawLine(begin1, end1);
            }
        }
    }

    public Cell Find(Func<Cell, bool> predicate)
    {
        for (int row = 0; row < _cells.GetLength(0); row++)
        {
            for (int col = 0; col < _cells.GetLength(1); col++)
            {
                var cell = this[row, col];
                if (predicate(cell))
                    return cell;
            }
        }

        return null;
    }

    private Graph<Cell> UpdateWalkability(IEnumerable<MeshCollider> colliders, float threshold)
    {
        var half = new Vector3(1 / (float)(scale * 2), 0.1f, 1 / (float)(scale * 2));
        foreach (var collider in colliders)
        {
            var min = collider.bounds.min;
            var max = collider.bounds.max;

            var begin = (col: ToIndex(min.x), row: ToIndex(min.z));
            var end = (col: ToIndex(max.x), row: ToIndex(max.z));
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

                    this[row, col].walkable = true;
                }
            }
        }

        var graph = new Graph<Cell>();
        foreach (var cell in _cells)
        {
            graph.nodes.Add(cell);
        }
        return graph;
    }

    private IEnumerable<Cell> Nears(Cell cell, bool includeOpposite = true)
    {
        var isLeftest = !(cell.row > 0);
        if (!isLeftest)
            yield return this[cell.row - 1, cell.col];

        var isRightest = !(cell.row < rows - 1);
        if (!isRightest)
            yield return this[cell.row + 1, cell.col];

        var isTopest = !(cell.col > 0);
        if (!isTopest)
            yield return this[cell.row, cell.col - 1];

        var isBottomest = !(cell.col < cols - 1);
        if (!isBottomest)
            yield return this[cell.row, cell.col + 1];

        if (includeOpposite)
        {
            if (!isLeftest && !isTopest)
                yield return this[cell.row - 1, cell.col - 1];

            if (!isRightest && !isTopest)
                yield return this[cell.row + 1, cell.col - 1];

            if (!isLeftest && !isBottomest)
                yield return this[cell.row - 1, cell.col + 1];

            if (!isRightest && !isBottomest)
                yield return this[cell.row + 1, cell.col + 1];
        }

        yield break;
    }

    private Graph<Region> UpdateRegion(int rows, int cols)
    {
        var regions = new Graph<Region>();
        var groups = _cells.GroupBy(x =>
        {
            var row = x.row / (this.rows / rows);
            var col = x.col / (this.cols / cols);

            return row * cols + col;
        }).ToDictionary(x => x.Key, x => x.ToList());

        var region = 0;
        foreach (var cells in groups.Values)
        {
            var hashCell = new HashSet<Cell>(cells);
            var hashSet = new HashSet<Cell>();
            var queue = new Queue<Cell>();
            while (true)
            {
                // region이 정해지지 않은 cell을 하나 추출
                var first = cells.FirstOrDefault(x => x.region == null);
                if (first == null)
                    break;

                var node = regions.nodes.Add(new Region { id = region, walkable = first.walkable });

                queue.Enqueue(first);
                hashSet.Add(first);

                while (queue.Count > 0)
                {
                    var pivot = queue.Dequeue();
                    pivot.region = node.data;

                    // 인접한 cell 중에 walkability가 같은 cell만 추출
                    var nears = Nears(pivot).Where(x => 
                    {
                        if (hashCell.Contains(x) == false)
                            return false;

                        if (x.region != null)
                            return false;

                        if (x.walkable != pivot.walkable)
                            return false;

                        return true;
                    });

                    foreach (var near in nears)
                    {
                        if (hashSet.Contains(near))
                            continue;

                        queue.Enqueue(near);
                        hashSet.Add(near);
                    }
                }

                region++;
            }
        }

        return regions;
    }

    public void UpdateCellGraph()
    {
        foreach (var node in cells.nodes)
        {
            foreach (var near in Nears(node.data))
            {
                cells.nodes[near].edges.Add(node);
                node.edges.Add(cells.nodes[near]);
            }
        }
    }

    public void UpdateRegionGraph()
    {
        var cached = cells.nodes.GroupBy(x => x.data.region).ToDictionary(x => x.Key, x => x.ToList());
        foreach (var node in regions.nodes)
        {
            // 인접한 region 연결
            var relations = cached[node.data].SelectMany(x => x.edges).GroupBy(x => x.data.region).Select(x => x.Key).Except(new[] { node.data }).ToList();
            foreach (var relation in relations)
            {
                node.edges.Add(regions.nodes[relation]);
                regions.nodes[relation].edges.Add(node);
            }

            var row = 0;
            var col = 0;
            cached[node.data].ForEach(x =>
            {
                row += x.data.row;
                col += x.data.col;
            });

            row = (int)(row / (float)cached[node.data].Count);
            col = (int)(col / (float)cached[node.data].Count);

            node.data.centroid = cached[node.data].OrderBy(x => Math.Pow(row - x.data.row, 2) + Math.Pow(col - x.data.col, 2)).First().data;
        }
    }

    public void Update(IEnumerable<MeshCollider> colliders, float threshold)
    {
        cells = UpdateWalkability(colliders, threshold);
        regions = UpdateRegion(10, 10);

        UpdateCellGraph();
        UpdateRegionGraph();
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
    public CellCollection cells { get; private set; }

    [SerializeField] public bool drawCells = false;
    [SerializeField] public bool drawEdges = false;
    [SerializeField] public bool drawRegionEdges = false;

    private void OnDrawGizmos()
    {
        if(drawCells)
            cells?.OnDrawCells();

        if (drawEdges)
            cells?.OnDrawEdges();

        if (drawRegionEdges)
            cells?.OnDrawRegionEdges();
    }

    public void Start()
    {
        cells = new CellCollection(width, height, scale);

        //sectors = new Sectors(this, 2, 2);
        //var sector = sectors[new FixVector3(50, 10)];
        //var nears = sector?.nears;

        var tiles = GetComponentsInChildren<Transform>().Except(new[] { this.GetComponent<Transform>() });
        foreach (var tile in tiles)
        {
            tile.gameObject.AddComponent<MeshCollider>();
        }

        cells.Update(tiles.Select(x => x.gameObject.GetComponent<MeshCollider>()), 1.0f);
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