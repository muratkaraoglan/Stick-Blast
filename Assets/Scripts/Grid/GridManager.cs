using System;
using System.Collections.Generic;
using System.Linq;
using Placable;
using Pooling;
using UnityEngine;
using Utils;

namespace Grid
{
    public class GridManager : Singleton<GridManager>
    {
        [SerializeField] private GridRenderer gridRenderer;
        [SerializeField] private Node nodePrefab;
        [SerializeField] private OccupiedCellPool occupiedCellPool;
        [Header("Grid Settings")] [SerializeField]
        private int width = 6;

        [SerializeField] private int height = 6;
        [SerializeField] private Color gridColor;

        private readonly float _cellSize = 3f;
        private Camera _mainCamera;
        private Dictionary<Vector3Int, Edge> _edgeMap = new();
        private Dictionary<Vector3, Node> _nodeMap = new();
        private HashSet<Vector3Int> _highlightedEdges = new();
        private CellChecker _cellChecker;
        private ShapePlacementManager _shapePlacementManager;

        private void Start()
        {
            _mainCamera = Camera.main;
            var edges = gridRenderer.InitializeGridRenderer(width, height, _cellSize, gridColor);
            InitializeNodes();
            _edgeMap = edges.ToDictionary(e => e.GetEdgePositionInt(), e => e);
            InitializeHelpers();
        }

        private void InitializeNodes()
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    var node = Instantiate(nodePrefab, transform);
                    var position = new Vector3(x * _cellSize, y * _cellSize, 0);
                    node.transform.localPosition = position;
                    node.SetSpriteColor(gridColor);
                    _nodeMap.Add(Vector3Int.FloorToInt(position), node);
                }
            }
        }

        private void InitializeHelpers()
        {
            _cellChecker = new CellChecker(_cellSize, _edgeMap,occupiedCellPool);
            _shapePlacementManager = new ShapePlacementManager(_edgeMap, _highlightedEdges, _mainCamera, _cellSize);
        }

        private void FindFullyOccupiedCells()
        {
            foreach (var key in _highlightedEdges)
            {
                var edge = _edgeMap[key];
                var edgePosition = edge.GetEdgePosition();
                _cellChecker.CheckCell(edgePosition, edge.Orientation);
            }
        }
        
        public bool CanPlaceShape(List<PlaceableShapeEdgeData> placeableEdges)
        {
            return _shapePlacementManager.TryPlaceShape(placeableEdges);
        }
        
        public void PlaceShape()
        {
            foreach (var key in _highlightedEdges)
            {
                var edge = _edgeMap[key];
                edge.SetEdgeIsOccupied(true);
                edge.ChangeHighlightAlpha(1f);
                Vector3Int nodeOne = Vector3Int.zero, nodeTwo = Vector3Int.zero;

                if (edge.Orientation == EdgeOrientation.Vertical)
                {
                    nodeOne = Vector3Int.FloorToInt(edge.GetEdgePosition() + _cellSize * .5f * Vector3.up);
                    nodeTwo = Vector3Int.FloorToInt(edge.GetEdgePosition() + _cellSize * .5f * Vector3.down);
                }
                else
                {
                    nodeOne = Vector3Int.FloorToInt(edge.GetEdgePosition() + _cellSize * .5f * Vector3.right);
                    nodeTwo = Vector3Int.FloorToInt(edge.GetEdgePosition() + _cellSize * .5f * Vector3.left);
                }

                _nodeMap[nodeOne].SetSpriteColor(Color.white);
                _nodeMap[nodeTwo].SetSpriteColor(Color.white);
            }

            FindFullyOccupiedCells();
            _highlightedEdges.Clear();
        }
        
    }

    public enum EdgeOrientation
    {
        Horizontal,
        Vertical
    }
}