using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KG
{
    public interface IPathFindable
    {
        public bool walkable { get; }
        public FixVector3 position { get; }
    }

    public partial class Graph<T> : IEnumerable<Graph<T>.Node>, IReadOnlyCollection<Graph<T>.Node> where T : IPathFindable
    {
        #region Node
        public class Node
        {
            public T data { get; private set; }
            public HashSet<Node> edges { get; private set; } = new HashSet<Node>();

            public Node(T data)
            {
                this.data = data;
            }
        }
        #endregion

        #region Path finding parameter
        private class AStarParams
        {
            public int G { get; set; } = 0;
            public int H { get; set; } = 0;
            public int F => G + H;
            public Node parent { get; set; } = null;
        }
        #endregion

        private readonly Dictionary<T, Node> _nodes = new Dictionary<T, Node>();
        private Dictionary<Node, AStarParams> _pathFindingParams;

        public int Count => _nodes.Count;

        public IEnumerator<Node> GetEnumerator() => _nodes.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _nodes.Values.GetEnumerator();


        public Node this[T key] => _nodes[key];

        public Node Add(T value)
        {
            if (_nodes.ContainsKey(value))
                return _nodes[value];

            return (_nodes[value] = new Node(value));
        }

        public Stack<T> Find(T begin, T end)
        {
            if (_pathFindingParams == null)
            {
                _pathFindingParams = _nodes.Values.ToDictionary(x => x, x => new AStarParams());
            }

            var result = new Stack<T>();
            var openedList = new List<Node>();
            var closedList = new List<Node>();

            openedList.Add(this[begin]);
            while (openedList.Count > 0 && closedList.Contains(this[end]) == false)
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

            if (closedList.Contains(this[end]) == false)
                return result;

            var tracked = this[end];
            while (tracked != this[begin])
            {
                result.Push(tracked.data);
                tracked = _pathFindingParams[tracked].parent;
            }
            return result;
        }
    }
}