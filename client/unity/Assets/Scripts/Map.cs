using FixMath.NET;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KG
{
    public partial class Map : MonoBehaviour
    {
        #region Region
        public class Region : IPathFindable
        {
            private readonly Map _map;

            public int id { get; private set; }
            public Cell centroid { get; set; }
            public HashSet<Unit> units { get; private set; } = new HashSet<Unit>();

            public FixVector2 position => centroid.position;
            public bool walkable { get; private set; }

            public Region(Map map, int id, bool walkable)
            {
                this._map = map;
                this.id = id;
                this.walkable = walkable;
            }

            public override string ToString()
            {
                return $"{base.ToString()}({id},{walkable})";
            }
        }
        #endregion

        #region Cell
        public class Cell : IPathFindable
        {
            private readonly Map _map;

            public int row { get; private set; }
            public int col { get; private set; }
            public Region region { get; set; }

            public FixVector2 position => new FixVector2(new Fix64(col) / new Fix64(_map.scale), new Fix64(row) / new Fix64(_map.scale));
            public FixVector2 center
            {
                get
                {
                    var position = this.position;
                    return new FixVector2(position.x + halfSize, position.y + halfSize);
                }
            }
            public bool walkable { get; private set; }

            public Fix64 size => _map.cellSize;
            public Fix64 halfSize => size / (Fix64.One * 2);
            public FixRect collisionBox => new FixRect(position, new FixVector2(size, size));

            public Cell(Map map, int row, int col, bool walkable)
            {
                this._map = map;
                this.row = row;
                this.col = col;
                this.walkable = walkable;
            }

            public Cell Near(Func<Cell, bool> func, Fix64? limit = null)
            {
                return _map.cells.Round(this, func, (pivot, x) =>
                {
                    if (limit == null)
                        return true;

                    if ((pivot.center - x.center).magnitude < limit.Value)
                        return true;

                    return false;
                });
            }

            public override string ToString()
            {
                return $"{base.ToString()}({row},{col},{walkable})";
            }
        }
        #endregion

        private Cell[] _cells;
        
        public Graph<Cell> cells { get; private set; } = new Graph<Cell>();
        public Graph<Region> regions { get; private set; } = new Graph<Region>();
        
        public int cols => width * scale;
        public int rows => height * scale;
        [field: Header("Properties"),SerializeField]
        public int width { get; private set; } = 192;
        [field: SerializeField]
        public int height { get; private set; } = 168;
        [field: SerializeField]
        public int scale { get; private set; } = 4;

        [field: SerializeField] 
        public int regionColDivider { get; private set; } = 10;
        [field: SerializeField] 
        public int regionRowDivider { get; private set; } = 10;
        public Fix64 cellSize => Fix64.One / new Fix64(scale);

        public static implicit operator Cell[](Map map) => map._cells;

        public Cell this[FixVector2 position]
        {
            get => this[ToIndex(position.y), ToIndex(position.x)];
            set => this[ToIndex(position.y), ToIndex(position.x)] = value;
        }

        public int Flatten(int row, int col)
        {
            return row * cols + col;
        }
        
        public Cell this[int row, int col]
        {
            get
            {
                var index = row * cols + col;
                if (index < 0 || index > _cells.Length - 1)
                    return null;
                
                return _cells[index];
            }
            set
            {
                var index = row * cols + col;
                if (index < 0 || index > _cells.Length - 1)
                    return;

                _cells[index] = value;
            }
        }

        private void CreateCells(int rows, int cols)
        {
            _cells = new Cell[rows * cols];
            
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var i = Flatten(row, col);
                    _cells[i] = new Cell(this, row, col, _walkability[i]);
                }
            }
        }
        
        private static Graph<Cell> CreateCellGraph(Cell[] cells)
        {
            var graph = new Graph<Cell>();
            foreach(var cell in cells)
            {
                graph.Add(cell);
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

                    var node = regions.Add(new Region(this, region, first.walkable));

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

        private void UpdateCellGraph()
        {
            foreach (var node in cells)
            {
                foreach (var near in Nears(node.data))
                {
                    cells[near].edges.Add(node);
                    node.edges.Add(cells[near]);
                }
            }
        }

        private void UpdateRegionGraph()
        {
            var cached = cells.GroupBy(x => x.data.region).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var node in regions)
            {
                // 인접한 region 연결
                var relations = cached[node.data].SelectMany(x => x.edges).GroupBy(x => x.data.region).Select(x => x.Key).Except(new[] { node.data }).ToList();
                foreach (var relation in relations)
                {
                    node.edges.Add(regions[relation]);
                    regions[relation].edges.Add(node);
                }

                // 무게중심 계산
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

        private int ToIndex(Fix64 value)
        {
            return (int)((float)value * this.scale);
        }
        
        private void Provisioning()
        {
            CreateCells(rows, cols);
            cells = CreateCellGraph(_cells);
            regions = UpdateRegion(regionRowDivider, regionColDivider);

            UpdateCellGraph();
            UpdateRegionGraph();
        }
        
        private void Awake()
        {
            Provisioning();
        }

        public void Start()
        {

        }

        public void Update()
        {

        }

        #region Precompute
        
        [SerializeField] public float _threshold = 1.0f;
        [SerializeField, HideInInspector] public bool[] _walkability;

        [ContextMenu("Update Walkability")]
        public void OnUpdateWalkability()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Update Walkability");
            UpdateWalkability();
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        private void UpdateWalkability()
        {
            var maxCollideHeights = Enumerable.Repeat(float.PositiveInfinity, rows * cols).ToArray();

            var tiles = GetComponentsInChildren<Transform>().Except(new[] {this.GetComponent<Transform>()});
            foreach (var tile in tiles)
            {
                if (tile.gameObject.GetComponent<MeshCollider>() == null)
                {
                    tile.gameObject.AddComponent<MeshCollider>();   
                }
                else
                {
                    var components = tile.gameObject.GetComponents<MeshCollider>();
                
                    for (int i = 1; i < components.Length; i++)
                        DestroyImmediate(components[i]);
                }
            }

            var colliders = tiles.Select(x => x.gameObject.GetComponent<MeshCollider>()).ToArray();

            var half = new Vector3(1 / (float) (scale * 2), 0.1f, 1 / (float) (scale * 2));
            foreach (var collider in colliders.OrderByDescending(x => x.bounds.max.y))
            {
                var min = collider.bounds.min;
                var max = collider.bounds.max;

                var begin = (col: ToIndex(min.x), row: ToIndex(min.z));
                var end = (col: ToIndex(max.x), row: ToIndex(max.z));
                for (int row = begin.row; row <= end.row; row++)
                {
                    for (int col = begin.col; col <= end.col; col++)
                    {
                        var center = new Vector3(col / (float) scale + half.x, 100.0f, row / (float) scale + half.z);
                        var hits = Physics.BoxCastAll(center, half, Vector3.down).Where(x => x.collider == collider).ToArray();
                        if (hits.Length == 0)
                            continue;

                        var hit = hits.FirstOrDefault();
                        var index = Flatten(row, col);
                        
                        if (index < 0 || rows * cols <= index)
                            continue;
                        
                        if (float.IsPositiveInfinity(maxCollideHeights[index]))
                        {
                            maxCollideHeights[index] = hit.point.y;
                        }
                        else
                        {
                            maxCollideHeights[index] = Mathf.Max(maxCollideHeights[index], hit.point.y);
                        }
                    }
                }
            }

            _walkability = maxCollideHeights.Select(x => x <= _threshold).ToArray();
        }

        #endregion
    }
}