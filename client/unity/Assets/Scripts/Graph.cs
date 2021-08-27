using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IPathFindable
{
    public bool walkable { get; }
    public FixVector3 position { get; }
}

public partial class Graph<T>
{
    public class Node
    {
        public T data { get; private set; }
        public HashSet<Node> edges { get; private set; } = new HashSet<Node>();

        public Node(T data)
        {
            this.data = data;
        }
    }

    public class NodeCollection : IEnumerable<Node>
    {
        private readonly Dictionary<T, Node> _nodes = new Dictionary<T, Node>();

        public IEnumerator<Node> GetEnumerator() => _nodes.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _nodes.Values.GetEnumerator();

        public int Count => _nodes.Count;

        public Node this[T key] => _nodes[key];

        public Node Add(T value)
        {
            if (_nodes.ContainsKey(value))
                return _nodes[value];

            return (_nodes[value] = new Node(value));
        }
    }

    public NodeCollection nodes { get; private set; } = new NodeCollection();

    public Node Find(Func<Node, bool> predicate) => nodes.FirstOrDefault(predicate);
}

public partial class Graph<T> where T : IPathFindable
{
    private class AStarParams
    {
        public int G { get; set; } = 0;
        public int H { get; set; } = 0;
        public int F => G + H;
        public Node parent { get; set; } = null;
    }

    private Dictionary<Node, AStarParams> _pathFindingParams;

    public Stack<T> Find(T begin, T end)
    {
        if (_pathFindingParams == null)
        {
            _pathFindingParams = nodes.ToDictionary(x => x, x => new AStarParams());
        }

        var result = new Stack<T>();
        var openedList = new List<Node>();
        var closedList = new List<Node>();

        openedList.Add(nodes[begin]);
        while (openedList.Count > 0 && closedList.Contains(nodes[end]) == false)
        {
            var current = openedList.First();
            openedList.Remove(current);
            closedList.Add(current);

            var nears = current.edges
                .Where(x => x.data.walkable)
                .Where(x => closedList.Contains(x) == false)
                .Where(x => openedList.Contains(x) == false)
                .ToList();

            foreach (var near in nears)
            {
                var param = _pathFindingParams[near];
                param.parent = current;
                param.G = (near.data.position - current.data.position).sqrMagnitude;
                param.H = (near.data.position - end.position).sqrMagnitude;
            }

            openedList.AddRange(nears);
            openedList.Sort((x, y) => _pathFindingParams[x].F.CompareTo(_pathFindingParams[y].F)); // OrderBy보다 빠른듯
        }

        if (closedList.Contains(nodes[end]) == false)
            return result;

        var tracked = nodes[end];
        while (tracked != nodes[begin])
        {
            result.Push(tracked.data);
            tracked = _pathFindingParams[tracked].parent;
        }
        return result;
    }
}