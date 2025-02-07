using System;
using System.Collections;
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
        private HashSet<Vector3Int> _highlightedEdges = new();
        private CellChecker _cellChecker;
        private ShapePlacementManager _shapePlacementManager;
        private NodeManager _nodeManager;

        private void Start()
        {
            _mainCamera = Camera.main;
            var edges = gridRenderer.InitializeGridRenderer(width, height, _cellSize, gridColor);
            _edgeMap = edges.ToDictionary(e => e.GetEdgePositionInt(), e => e);
            InitializeHelpers();
        }

        private void InitializeHelpers()
        {
            _cellChecker = new CellChecker(_cellSize, width, height, _edgeMap, occupiedCellPool);
            _shapePlacementManager = new ShapePlacementManager(_edgeMap, _highlightedEdges, _mainCamera, _cellSize);
            _nodeManager = new NodeManager(width, height, _cellSize, gridColor, nodePrefab, transform);
        }

        private void FindFullyOccupiedCells()
        {
            CheckHighlightedEdges();

            var edgeClearList = _cellChecker.DestroyCells();

            ResetEdges();

            FixOccupiedNodes();

            return;

            void CheckHighlightedEdges()
            {
                foreach (var key in _highlightedEdges)
                {
                    var edge = _edgeMap[key];
                    var edgePosition = edge.GetEdgePosition();
                    _cellChecker.CheckCell(edgePosition, edge.Orientation);
                }
            }

            void ResetEdges()
            {
                foreach (var edgePosition in edgeClearList)
                {
                    var edgeKey = Vector3Int.FloorToInt(edgePosition);
                    var edge = _edgeMap[edgeKey];
                    edge.ResetEdge();
                    _nodeManager.UnOccupyNode(edgePosition, edge.Orientation);
                }
            }

            void FixOccupiedNodes()
            {
                foreach (var edgekvp in _edgeMap.Where(kvp => kvp.Value.IsEdgeOccupied()))
                {
                    _nodeManager.OccupyNode(edgekvp.Value.GetEdgePosition(), edgekvp.Value.Orientation);
                }
            }
        }

        public bool CanPlaceShape(List<PlaceableShapeEdgeData> placeableEdges)
        {
            return _shapePlacementManager.TryPlaceShape(placeableEdges);
        }

        public void PlaceShape()
        {
            if (!_canPlace) return;
            _canPlace = false;
            foreach (var key in _highlightedEdges)
            {
                var edge = _edgeMap[key];
                edge.SetEdgeIsOccupied(true);
                edge.ChangeHighlightAlpha(1f);
                _nodeManager.OccupyNode(edge.GetEdgePosition(), edge.Orientation);
            }

            FindFullyOccupiedCells();
            _highlightedEdges.Clear();
            StartCoroutine(DelayPlace());
        }

        private bool _canPlace = true;

        private IEnumerator DelayPlace()
        {
            yield return new WaitForSeconds(.3f);
            _canPlace = true;
        }
    }

    public enum EdgeOrientation
    {
        Horizontal,
        Vertical
    }
}