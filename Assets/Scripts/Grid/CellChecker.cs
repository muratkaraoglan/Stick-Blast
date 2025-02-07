using System.Collections.Generic;
using DG.Tweening;
using Pooling;
using UnityEngine;
using System.Linq;

namespace Grid
{
    public class CellChecker
    {
        private readonly int _width;
        private readonly int _height;
        private readonly float _cellSize;
        private readonly float _cellScale = 20f;
        private readonly float _cellScaleTimeInSeconds = .15f;
        private readonly OccupiedCellPool _occupiedCellPool;
        private readonly Dictionary<Vector3Int, GameObject> _occupiedCells = new();
        private readonly Dictionary<Vector3Int, Edge> _edgeMap;
        private HashSet<Vector3Int> _destroyCells = new();

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

        private bool _isDestroying;

        public List<Vector3> DestroyCells()
        {
            _isDestroying = true;
            var cellCenters = GetCellCenters();
            var edgeClearList = GetEdgeClearList(cellCenters);

            _destroyCells.Clear();
            _isDestroying = false;
            return edgeClearList;
        }

        private List<Vector3> GetCellCenters()
        {
            var cellCenters = new List<Vector3>();

            foreach (var cellKey in _destroyCells)
            {
                cellCenters.Add(_occupiedCells[cellKey].transform.position);
                ReleaseCellToPool(cellKey);
            }

            return cellCenters;
        }

        private void ReleaseCellToPool(Vector3Int cellKey)
        {
            _occupiedCellPool.Pool.Release(_occupiedCells[cellKey]);
            _occupiedCells.Remove(cellKey);
        }

        private List<Vector3> GetEdgeClearList(List<Vector3> cellCenters)
        {
            var edgeClearList = new List<Vector3>();

            foreach (var cellCenter in cellCenters)
            {
                CheckAndAddEdgePosition(cellCenter, Vector3.up, edgeClearList);
                CheckAndAddEdgePosition(cellCenter, Vector3.down, edgeClearList);
                CheckAndAddEdgePosition(cellCenter, Vector3.left, edgeClearList);
                CheckAndAddEdgePosition(cellCenter, Vector3.right, edgeClearList);
            }

            return edgeClearList;
        }

        private void CheckAndAddEdgePosition(Vector3 cellCenter, Vector3 direction, List<Vector3> edgeClearList)
        {
            var neighborPosition = cellCenter + direction * _cellSize;

            if (!IsCellOccupied(neighborPosition))
            {
                var edgePosition = cellCenter + direction * _cellSize * 0.5f;
                edgeClearList.Add(edgePosition);
            }
        }

        private bool IsCellOccupied(Vector3 position)
        {
            return _occupiedCells.ContainsKey(Vector3Int.FloorToInt(position));
        }

        private Vector3Int GetNeighborEdgePosition(Vector3 edgePosition, Vector3 direction)
        {
            return Vector3Int.FloorToInt(edgePosition + direction * _cellSize);
        }

        private bool IsNeighborEdgeOccupied(Vector3Int neighborPosition)
        {
            if (_edgeMap.TryGetValue(neighborPosition, out Edge neighborEdge))
            {
                return neighborEdge.IsEdgeOccupied();
            }

            return false;
        }

        private void CheckHorizontalCells(Vector3 edgePosition)
        {
            var rightNeighbor = GetNeighborEdgePosition(edgePosition, Vector3.right);
            var rightUpNeighbor = GetNeighborEdgePosition(edgePosition, new Vector3(.5f, .5f, 0));
            var rightDownNeighbor = GetNeighborEdgePosition(edgePosition, new Vector3(.5f, -.5f, 0));
            var leftNeighbor = GetNeighborEdgePosition(edgePosition, Vector3.left);
            var leftUpNeighbor = GetNeighborEdgePosition(edgePosition, new Vector3(-.5f, .5f, 0));
            var leftDownNeighbor = GetNeighborEdgePosition(edgePosition, new Vector3(-.5f, -.5f, 0));

            if (IsNeighborEdgeOccupied(rightNeighbor)
                && IsNeighborEdgeOccupied(rightUpNeighbor)
                && IsNeighborEdgeOccupied(rightDownNeighbor)
               )
            {
                SpawnCell(edgePosition + _cellSize * 0.5f * Vector3.right);
            }

            if (IsNeighborEdgeOccupied(leftNeighbor)
                && IsNeighborEdgeOccupied(leftUpNeighbor)
                && IsNeighborEdgeOccupied(leftDownNeighbor))
            {
                SpawnCell(edgePosition + _cellSize * 0.5f * Vector3.left);
            }
        }

        private void CheckVerticalCells(Vector3 edgePosition)
        {
            var upNeighbor = GetNeighborEdgePosition(edgePosition, Vector3.up);
            var upRightNeighbor = GetNeighborEdgePosition(edgePosition, new Vector3(.5f, .5f, 0));
            var upLeftNeighbor = GetNeighborEdgePosition(edgePosition, new Vector3(-.5f, .5f, 0));
            var downNeighbor = GetNeighborEdgePosition(edgePosition, Vector3.down);
            var downRightNeighbor = GetNeighborEdgePosition(edgePosition, new Vector3(.5f, -.5f, 0));
            var downLeftNeighbor = GetNeighborEdgePosition(edgePosition, new Vector3(-.5f, -.5f, 0));

            if (IsNeighborEdgeOccupied(upNeighbor)
                && IsNeighborEdgeOccupied(upRightNeighbor)
                && IsNeighborEdgeOccupied(upLeftNeighbor)
               )
            {
                SpawnCell(edgePosition + _cellSize * 0.5f * Vector3.up);
            }

            if (IsNeighborEdgeOccupied(downNeighbor)
                && IsNeighborEdgeOccupied(downRightNeighbor)
                && IsNeighborEdgeOccupied(downLeftNeighbor))
            {
                SpawnCell(edgePosition + _cellSize * 0.5f * Vector3.down);
            }
        }

        private void SpawnCell(Vector3 position)
        {
            if (_isDestroying) return;
            var pos = Vector3Int.FloorToInt(position);
            if(_occupiedCells.ContainsKey(pos)) return;
            var go = _occupiedCellPool.Pool.Get();
            go.transform.position = position;
            go.transform.DOScale(_cellScale, _cellScaleTimeInSeconds);
            _occupiedCells.TryAdd(pos, go);
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