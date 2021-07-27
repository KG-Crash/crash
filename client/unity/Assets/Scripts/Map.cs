using FixMath.NET;
using Game;
using System.Collections;
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
        var top = 5.0f;
        foreach (var collider in tiles.Select(x => x.gameObject.GetComponent<MeshCollider>()))
        {
            var min = collider.bounds.min;
            var max = collider.bounds.max;

            var begin = (col: walkabilityTable.ToIndex((Fix64)min.x), row: walkabilityTable.ToIndex((Fix64)min.z));
            var end = (col: walkabilityTable.ToIndex((Fix64)max.x), row: walkabilityTable.ToIndex((Fix64)max.z));
            for (int row = begin.row; row <= end.row; row++)
            {
                for (int col = begin.col; col <= end.col; col++)
                {
                    var margin = 1 / (float)(scale * 2);
                    var ray = new Ray(new Vector3(col / (float)scale + margin, top, row / (float)scale + margin), Vector3.down);

                    if (collider.Raycast(ray, out var hit, top * 2.0f) && hit.point.y < threshold)
                        walkabilityTable[row, col] = true;
                }
            }
        }
    }

    public void Update()
    {
        
    }
}


public class Node
{
    public Node(uint x, uint y) { _x = x; _y = y; }

    //public bool _isBlocked { get; private set; }
    public uint _x { get; private set; }
    public uint _y { get; private set; }
    public uint _G { get; set; } = 0;
    public uint _H { get; set; } = 0;
    public uint _F { get { return _G + _H; } }
    public Node _parent { get; set; } = null;

}
public static class Astar
{    
    public static List<Node> GetRoot(FixVector3 beg, FixVector3 end, bool[,] maps, Node[,] nodeMap)
    {
        List<Node> resultList;
        List<Node> openList, closedList;
        Node begNode, endNode, curNode;

        begNode = nodeMap[(uint)beg.x, (uint)beg.z];
        endNode = nodeMap[(uint)end.x, (uint)end.z];

        openList = new List<Node>() { begNode };
        closedList = new List<Node>();
        resultList = new List<Node>();

        while (openList.Count > 0)
        {
            curNode = openList[0];
            foreach (var openNode in openList)
                if (openNode._F <= curNode._F && openNode._H < curNode._H)
                    curNode = openNode;

            openList.Remove(curNode);
            closedList.Add(curNode);

            if (curNode == endNode) 
            {
                Node tempNode = endNode;
                while (tempNode != begNode)
                {
                    resultList.Add(tempNode);
                    tempNode = tempNode._parent;
                }
                resultList.Add(begNode);
                resultList.Reverse();

                return resultList;
            }

            AddOpenList(curNode._x + 1, curNode._y + 1, maps, openList, closedList, nodeMap, endNode, curNode);
            AddOpenList(curNode._x - 1, curNode._y + 1, maps, openList, closedList, nodeMap, endNode, curNode);
            AddOpenList(curNode._x - 1, curNode._y - 1, maps, openList, closedList, nodeMap, endNode, curNode);
            AddOpenList(curNode._x + 1, curNode._y - 1, maps, openList, closedList, nodeMap, endNode, curNode);

            AddOpenList(curNode._x, curNode._y + 1, maps, openList, closedList, nodeMap, endNode, curNode);
            AddOpenList(curNode._x + 1, curNode._y, maps, openList, closedList, nodeMap, endNode, curNode);
            AddOpenList(curNode._x, curNode._y - 1, maps, openList, closedList, nodeMap, endNode, curNode);
            AddOpenList(curNode._x - 1, curNode._y, maps, openList, closedList, nodeMap, endNode, curNode);
        }

        return null;
    }

    private static void AddOpenList(uint x, uint y, bool[,] maps, List<Node> openlist, List<Node> closedList, Node[,] nodeMap, Node endNode, Node curNode)
    {
        if (x >= 0 && x < maps.GetLength(1) && y >= 0 && y < maps.GetLength(0) && !maps[x,y]
            && !closedList.Contains(nodeMap[y,x]))
        {
            
            if (maps[y, curNode._x] && maps[curNode._y, x]) return;
           
            if (maps[y, curNode._x] || maps[curNode._y, x]) return;

            Node neighbor = nodeMap[y, x];
            uint moveCost = curNode._G + (uint)(((curNode._x - x) == 0 || (curNode._y - y) == 0) ? 10 : 14);

            if (moveCost < neighbor._G || !openlist.Contains(neighbor))
            {
                neighbor._G = moveCost;
                neighbor._H = (uint)(Mathf.Abs(neighbor._x - endNode._x) + Mathf.Abs(neighbor._y - endNode._y)) * 10;
                neighbor._parent = curNode;

                openlist.Add(neighbor);
            }
        }
    }

    private static Vector2Int ConvertPositiontoGrid()
    {
        return new Vector2Int();
    }

    private static void InitNodeMap(Node[,] nodeMap)
    {

    }
}