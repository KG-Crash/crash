using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Graph<T>
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