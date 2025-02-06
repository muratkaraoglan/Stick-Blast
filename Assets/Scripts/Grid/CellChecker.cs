using System.Collections.Generic;
using DG.Tweening;
using Pooling;
using UnityEngine;

namespace Grid
{
    public class CellChecker
    {
        private readonly float _cellSize;
        private Dictionary<Vector3Int, Edge> _edgeMap;
        private readonly OccupiedCellPool _occupiedCellPool;
        private HashSet<Vector3Int> _occupiedCells = new();
        public CellChecker(float cellSize, Dictionary<Vector3Int, Edge> edgeMap, OccupiedCellPool pool)
        {
            _cellSize = cellSize;
            _edgeMap = edgeMap;
            _occupiedCellPool = pool;
        }

        public void CheckCell(Vector3 edgePosition, EdgeOrientation edgeOrientation)
        {
            if (edgeOrientation == EdgeOrientation.Vertical)
            {
                CheckHorizontalCells(edgePosition);
            }
            else
            {
                CheckVerticalCells(edgePosition);
            }
        }

        private Vector3Int GetNeighborPosition(Vector3 edgePosition, Vector3 direction)
        {
            return Vector3Int.FloorToInt(edgePosition + direction * _cellSize);
        }

        private bool IsNeighborOccupied(Vector3Int neighborPosition)
        {
            if (_edgeMap.TryGetValue(neighborPosition, out Edge neighborEdge))
            {
                return neighborEdge.IsEdgeOccupied();
            }

            return false;
        }

        private void CheckHorizontalCells(Vector3 edgePosition)
        {
            var rightNeighbor = GetNeighborPosition(edgePosition, Vector3.right);
            var rightUpNeighbor = GetNeighborPosition(edgePosition, new Vector3(.5f, .5f, 0));
            var rightDownNeighbor = GetNeighborPosition(edgePosition, new Vector3(.5f, -.5f, 0));
            var leftNeighbor = GetNeighborPosition(edgePosition, Vector3.left);
            var leftUpNeighbor = GetNeighborPosition(edgePosition, new Vector3(-.5f, .5f, 0));
            var leftDownNeighbor = GetNeighborPosition(edgePosition, new Vector3(-.5f, -.5f, 0));

            if (IsNeighborOccupied(rightNeighbor)
                && IsNeighborOccupied(rightUpNeighbor)
                && IsNeighborOccupied(rightDownNeighbor)
               )
            {
                InstantiateCell(edgePosition + _cellSize * 0.5f * Vector3.right);
            }

            if (IsNeighborOccupied(leftNeighbor)
                && IsNeighborOccupied(leftUpNeighbor)
                && IsNeighborOccupied(leftDownNeighbor))
            {
                InstantiateCell(edgePosition + _cellSize * 0.5f * Vector3.left);
            }
        }

        private void CheckVerticalCells(Vector3 edgePosition)
        {
            var upNeighbor = GetNeighborPosition(edgePosition, Vector3.up);
            var upRightNeighbor = GetNeighborPosition(edgePosition, new Vector3(.5f, .5f, 0));
            var upLeftNeighbor = GetNeighborPosition(edgePosition, new Vector3(-.5f, .5f, 0));
            var downNeighbor = GetNeighborPosition(edgePosition, Vector3.down);
            var downRightNeighbor = GetNeighborPosition(edgePosition, new Vector3(.5f, -.5f, 0));
            var downLeftNeighbor = GetNeighborPosition(edgePosition, new Vector3(-.5f, -.5f, 0));

            if (IsNeighborOccupied(upNeighbor)
                && IsNeighborOccupied(upRightNeighbor)
                && IsNeighborOccupied(upLeftNeighbor)
               )
            {
                InstantiateCell(edgePosition + _cellSize * 0.5f * Vector3.up);
            }

            if (IsNeighborOccupied(downNeighbor)
                && IsNeighborOccupied(downRightNeighbor)
                && IsNeighborOccupied(downLeftNeighbor))
            {
                InstantiateCell(edgePosition + _cellSize * 0.5f * Vector3.down);
            }
        }

        private void InstantiateCell(Vector3 position)
        {
            var go = _occupiedCellPool.Pool.Get();
            go.transform.position = position;
            go.transform.DOScale(20f, .15f);
        }
    }
}