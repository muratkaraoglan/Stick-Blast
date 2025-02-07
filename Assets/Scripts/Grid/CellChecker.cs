using System.Collections.Generic;
using DG.Tweening;
using Pooling;
using UnityEngine;
using System.Linq;

namespace Grid
{
    public class CellChecker
    {
        private readonly float _cellSize;
        private Dictionary<Vector3Int, Edge> _edgeMap;
        private readonly OccupiedCellPool _occupiedCellPool;
        private readonly Dictionary<Vector3Int, GameObject> _occupiedCells = new();
        private readonly float _cellScale = 20f;
        private readonly float _cellScaleTimeInSeconds = .15f;
        private HashSet<Vector3Int> _destroyCells = new();
        private int _width;
        private int _height;

        public CellChecker(float cellSize, int width, int height, Dictionary<Vector3Int, Edge> edgeMap,
            OccupiedCellPool pool)
        {
            _cellSize = cellSize;
            _width = width;
            _height = height;
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

        public void DestroyCells()
        {
            foreach (var cellKey in _destroyCells)
            {
                _occupiedCellPool.Pool.Release(_occupiedCells[cellKey]);
                _occupiedCells.Remove(cellKey);
            }

            _destroyCells.Clear();
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
                SpawnCell(edgePosition + _cellSize * 0.5f * Vector3.right);
            }

            if (IsNeighborOccupied(leftNeighbor)
                && IsNeighborOccupied(leftUpNeighbor)
                && IsNeighborOccupied(leftDownNeighbor))
            {
                SpawnCell(edgePosition + _cellSize * 0.5f * Vector3.left);
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
                SpawnCell(edgePosition + _cellSize * 0.5f * Vector3.up);
            }

            if (IsNeighborOccupied(downNeighbor)
                && IsNeighborOccupied(downRightNeighbor)
                && IsNeighborOccupied(downLeftNeighbor))
            {
                SpawnCell(edgePosition + _cellSize * 0.5f * Vector3.down);
            }
        }

        private void SpawnCell(Vector3 position)
        {
            var go = _occupiedCellPool.Pool.Get();
            go.transform.position = position;
            go.transform.DOScale(_cellScale, _cellScaleTimeInSeconds);
            _occupiedCells.TryAdd(Vector3Int.FloorToInt(position), go);
            CheckFullRowColumn(Vector3Int.FloorToInt(position));
        }

        private void CheckFullRowColumn(Vector3Int lastCellPosition)
        {
            CheckFullRow(lastCellPosition.x);
            CheckFullColumn(lastCellPosition.y);
        }

        private void CheckFullRow(int row)
        {
            if (IsRowFull(row))
            {
                AddRowCellsToDestroyList(row);
            }
        }

        private void CheckFullColumn(int column)
        {
            if (IsColumnFull(column))
            {
                AddColumnCellsToDestroyList(column);
            }
        }

        private bool IsRowFull(int row)
        {
            return CountOccupiedRowCells(row) == _width;
        }

        private bool IsColumnFull(int column)
        {
            return CountOccupiedColumnCells(column) == _height;
        }

        private int CountOccupiedRowCells(int row)
        {
            return _occupiedCells.Keys.Count(pos => pos.x == row);
        }

        private int CountOccupiedColumnCells(int column)
        {
            return _occupiedCells.Keys.Count(pos => pos.y == column);
        }

        private void AddRowCellsToDestroyList(int row)
        {
            var cellsInRow = _occupiedCells.Where(kvp => kvp.Key.x == row);
            AddCellsToDestroyList(cellsInRow);
        }

        private void AddColumnCellsToDestroyList(int column)
        {
            var cellsInColumn = _occupiedCells.Where(kvp => kvp.Key.y == column);
            AddCellsToDestroyList(cellsInColumn);
        }

        private void AddCellsToDestroyList(IEnumerable<KeyValuePair<Vector3Int, GameObject>> cells)
        {
            foreach (var kvp in cells)
            {
                _destroyCells.Add(kvp.Key);
            }
        }
    }
}