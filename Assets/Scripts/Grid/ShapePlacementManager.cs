using System.Collections.Generic;
using Placable;
using UnityEngine;

namespace Grid
{
    public class ShapePlacementManager
    {
        private readonly Dictionary<Vector3Int, Edge> _edgeMap;
        private readonly HashSet<Vector3Int> _highlightedEdges;
        private readonly Camera _mainCamera;
        private readonly float _cellSize;
        
        public ShapePlacementManager(Dictionary<Vector3Int, Edge> edgeMap, HashSet<Vector3Int> highlightedEdges,
            Camera mainCamera, float cellSize)
        {
            _edgeMap = edgeMap;
            _highlightedEdges = highlightedEdges;
            _mainCamera = mainCamera;
            _cellSize = cellSize;
        }

        public bool TryPlaceShape(List<PlaceableShapeEdgeData> placeableEdges)
        {
            ClearHighlights();

            if (!ValidateEdgePlacements(placeableEdges))
                return false;

            ShowHighlights(true);
            return true;
        }

        private void ClearHighlights()
        {
            foreach (var point in _highlightedEdges)
            {
                _edgeMap[point].ShowHighlight(false);
            }

            _highlightedEdges.Clear();
        }

        private bool ValidateEdgePlacements(List<PlaceableShapeEdgeData> placeableEdges)
        {
            foreach (var edgeData in placeableEdges)
            {
                var edgePosition = CalculateEdgePosition(edgeData);

                if (!IsValidEdgePlacement(edgePosition, edgeData.orientation))
                    return false;

                _highlightedEdges.Add(edgePosition);
            }

            return _highlightedEdges.Count == placeableEdges.Count;
        }

        private bool IsValidEdgePlacement(Vector3Int edgePosition, EdgeOrientation orientation)
        {
            return _edgeMap.TryGetValue(edgePosition, out Edge edge) &&
                   edge.Orientation == orientation &&
                   !edge.IsEdgeOccupied();
        }

        private void ShowHighlights(bool show)
        {
            foreach (var point in _highlightedEdges)
            {
                _edgeMap[point].ShowHighlight(show);
            }
        }

        private Vector3Int CalculateEdgePosition(PlaceableShapeEdgeData edgeData)
        {
            var worldPosition = _mainCamera.ScreenToWorldPoint(edgeData.transform.position);
            return edgeData.orientation == EdgeOrientation.Vertical
                ? CalculateVerticalEdgePosition(worldPosition)
                : CalculateHorizontalEdgePosition(worldPosition);
        }

        private Vector3Int CalculateVerticalEdgePosition(Vector3 worldPosition)
        {
            float snappedX = SnapToGrid(worldPosition.x);
            float normalizedY = worldPosition.y - 1f;
            float snappedY = SnapToGrid(normalizedY) + 1f;

            return new Vector3Int(
                Mathf.RoundToInt(snappedX),
                Mathf.RoundToInt(snappedY),
                0
            );
        }

        private Vector3Int CalculateHorizontalEdgePosition(Vector3 worldPosition)
        {
            float normalizedX = worldPosition.x - 1f;
            float snappedX = SnapToGrid(normalizedX) + 1f;
            float snappedY = SnapToGrid(worldPosition.y);

            return new Vector3Int(
                Mathf.RoundToInt(snappedX),
                Mathf.RoundToInt(snappedY),
                0
            );
        }

        private float SnapToGrid(float value) => Mathf.Round(value / _cellSize) * _cellSize;
    }
}