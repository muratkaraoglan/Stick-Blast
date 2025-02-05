using System;
using System.Collections.Generic;
using System.Linq;
using Placable;
using UnityEngine;
using Utils;

namespace Grid
{
    public class GridManager : Singleton<GridManager>
    {
        [SerializeField] private GridRenderer gridRenderer;
        [SerializeField] private Node nodePrefab;

        [Header("Grid Settings")] [SerializeField]
        private int width = 6;

        [SerializeField] private int height = 6;
        [SerializeField] private Color gridColor = Color.white;
        private readonly float _cellSize = 3f;

        private Dictionary<Vector3Int, Edge> _edgeMap = new();
        [SerializeField] private List<Vector3Int> edgePoints = new();

        private void Start()
        {
            var edges = gridRenderer.InitializeGridRenderer(width, height, _cellSize, gridColor);
            InitializeNodes();
            _edgeMap = edges.ToDictionary(e => e.GetEdgePositionInt(), e => e);
            edgePoints = _edgeMap.Keys.ToList();
        }

        private void InitializeNodes()
        {
            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    var node = Instantiate(nodePrefab, transform);

                    node.transform.localPosition = new Vector3(x * _cellSize, y * _cellSize, 0);
                    node.SetSpriteColor(gridColor);
                }
            }
        }

        public void CheckPlaceArea(List<PlaceableShapeEdgeData> placedEdges)
        {
            for (int i = 0; i < placedEdges.Count; i++)
            {
                var data = placedEdges[i];
                var worldPosition = Camera.main.ScreenToWorldPoint(data.transform.position);
                if (data.orientation == EdgeOrientation.Vertical)
                {
                    float snappedX = Mathf.Round(worldPosition.x / _cellSize) * _cellSize;
                    float normalizedY = worldPosition.y - 1f;//dikey eksendeki baslangic degeri 1 oldugu icin 1 cıkarip hesaplamalara 0 ile basliyor.
                    float snappedY = Mathf.Round(normalizedY / _cellSize) * _cellSize + 1f;

                    int x = Mathf.RoundToInt(snappedX);
                    int y = Mathf.RoundToInt(snappedY);
                    Vector3Int edgePosition = new Vector3Int(x, y, 0);

                    edgePosition.z = 0;
                    floorPoint = edgePosition;
                    if (_edgeMap.TryGetValue(edgePosition, out Edge edge) && edge.Orientation == data.orientation)
                    {
                        point = edge.GetEdgePosition();
                    }
                    else
                    {
                        point = Vector3.zero;
                    }
                }
                else
                {
                    float normalizedX = worldPosition.x - 1f;//yatay eksendeki baslangic degeri 1 oldugu icin 1 cıkarip hesaplamalara 0 ile basliyor.
                    float snappedX = Mathf.Round(normalizedX / _cellSize) * _cellSize + 1;
                    float snappedY = Mathf.Round(worldPosition.y / _cellSize) * _cellSize;
                    
                    int x = Mathf.RoundToInt(snappedX);
                    int y = Mathf.RoundToInt(snappedY);
                    Vector3Int edgePosition = new Vector3Int(x, y, 0);

                    edgePosition.z = 0;
                    if (_edgeMap.TryGetValue(edgePosition, out Edge edge) && edge.Orientation == data.orientation)
                    {
                        point = edge.GetEdgePosition();
                    }
                    else
                    {
                        point = Vector3.zero;
                    }

                }
            }
        }

        [SerializeField] private bool draw;
        [SerializeField] private float drawRadius = .2f;
        private Vector3 point;
        private Vector3 floorPoint;

        private void OnDrawGizmos()
        {
            if (!draw) return;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(floorPoint, drawRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(point, drawRadius);
        }
    }

    public enum EdgeOrientation
    {
        Horizontal,
        Vertical
    }
}