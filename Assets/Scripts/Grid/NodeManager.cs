using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class NodeManager
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Color _gridColor;
        private readonly Dictionary<Vector3Int, Node> _nodeMap = new();
        private readonly Node _nodePrefab;
        private readonly Transform _nodeParent;
        private readonly float _cellSize;

        public NodeManager(int width, int height, float cellSize, Color gridColor, Node nodePrefab,
            Transform nodeParent)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _gridColor = gridColor;
            _nodePrefab = nodePrefab;
            _nodeParent = nodeParent;
            InitializeNodes();
        }

        private void InitializeNodes()
        {
            for (int x = 0; x <= _width; x++)
            {
                for (int y = 0; y <= _height; y++)
                {
                    var node = Object.Instantiate(_nodePrefab, _nodeParent);
                    var position = new Vector3(x * _cellSize, y * _cellSize, 0);
                    node.transform.localPosition = position;
                    node.SetSpriteColor(_gridColor);
                    _nodeMap.Add(Vector3Int.FloorToInt(position), node);
                }
            }
        }

        public void OccupyNode(Vector3 edgePosition, EdgeOrientation edgeOrientation)
        {
            ChangeNodeState(edgePosition, edgeOrientation, Color.white);
        }

        public void UnOccupyNode(Vector3 edgePosition, EdgeOrientation edgeOrientation)
        {
            ChangeNodeState(edgePosition, edgeOrientation, _gridColor);
        }

        private void ChangeNodeState(Vector3 edgePosition, EdgeOrientation edgeOrientation, Color nodeColor)
        {
            Vector3Int nodeOne = Vector3Int.zero, nodeTwo = Vector3Int.zero;

            if (edgeOrientation == EdgeOrientation.Vertical)
            {
                nodeOne = CalculateNodePosition(edgePosition, .5f * Vector3.up);
                nodeTwo = CalculateNodePosition(edgePosition, .5f * Vector3.down);
            }
            else
            {
                nodeOne = CalculateNodePosition(edgePosition, .5f * Vector3.right);
                nodeTwo = CalculateNodePosition(edgePosition, .5f * Vector3.left);
            }

            _nodeMap[nodeOne].SetSpriteColor(nodeColor);
            _nodeMap[nodeTwo].SetSpriteColor(nodeColor);
        }

        private Vector3Int CalculateNodePosition(Vector3 basePosition, Vector3 direction)
        {
            return Vector3Int.FloorToInt(basePosition + direction * _cellSize);
        }
    }
}