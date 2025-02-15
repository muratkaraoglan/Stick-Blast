using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grid
{
    public class GridRenderer : MonoBehaviour
    {
        [SerializeField] private float cameraPadding = 4f;
        [SerializeField] private Material gridMaterial;
        [SerializeField] private Edge verticalEdgePrefab;
        [SerializeField] private Edge horizontalEdgePrefab;
        private int _width;
        private int _height;
        private float _cellSize;
        private float _lineWidth = .25f;
        private Color _blockColor = Color.white;
        private List<Edge> _edgeList;

        public List<Edge> InitializeGridRenderer(int width, int height, float cellSize, Color gridColor)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _blockColor = gridColor;
            var totalEdgeCount = 2 * (width * height) + width + height;
            _edgeList = new List<Edge>(totalEdgeCount);
            // CreateGrid();
            PrepareGrid();
            SetupCamera();
            return _edgeList;
        }

        private void CreateGrid()
        {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uv = new List<Vector2>();

            // Dikey çizgiler
            for (int x = 0; x <= _width; x++)
            {
                vertices.Add(new Vector3(x * _cellSize - _lineWidth / 2, 0, 0)); // sol alt
                vertices.Add(new Vector3(x * _cellSize + _lineWidth / 2, 0, 0)); // sağ alt
                vertices.Add(new Vector3(x * _cellSize - _lineWidth / 2, _height * _cellSize, 0)); // sol üst
                vertices.Add(new Vector3(x * _cellSize + _lineWidth / 2, _height * _cellSize, 0)); // sağ üst

                int vCount = vertices.Count;
                triangles.Add(vCount - 4); // sol alt
                triangles.Add(vCount - 2); // sol üst
                triangles.Add(vCount - 3); // sağ alt

                triangles.Add(vCount - 2); // sol üst
                triangles.Add(vCount - 1); // sağ üst
                triangles.Add(vCount - 3); // sağ alt

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
            }

            // Yatay çizgiler
            for (int y = 0; y <= _height; y++)
            {
                vertices.Add(new Vector3(0, y * _cellSize - _lineWidth / 2, 0)); // sol alt
                vertices.Add(new Vector3(_width * _cellSize, y * _cellSize - _lineWidth / 2, 0)); // sağ alt
                vertices.Add(new Vector3(0, y * _cellSize + _lineWidth / 2, 0)); // sol üst
                vertices.Add(new Vector3(_width * _cellSize, y * _cellSize + _lineWidth / 2, 0)); // sağ üst

                int vCount = vertices.Count;
                triangles.Add(vCount - 4); // sol alt
                triangles.Add(vCount - 2); // sol üst
                triangles.Add(vCount - 3); // sağ alt

                triangles.Add(vCount - 2); // sağ alt
                triangles.Add(vCount - 1); // sol üst
                triangles.Add(vCount - 3); // sağ üst

                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(1, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshFilter.mesh = mesh;
            gridMaterial.SetColor("_BaseColor", _blockColor);
            meshRenderer.material = gridMaterial;
            gameObject.isStatic = true;
        }

        private void PrepareGrid()
        {
            for (int y = 0; y <= _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    CreateEdge(horizontalEdgePrefab, x, y, EdgeOrientation.Horizontal);
                }
            }

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x <= _width; x++)
                {
                    CreateEdge(verticalEdgePrefab, x, y, EdgeOrientation.Vertical);
                }
            }
        }

        private void CreateEdge(Edge edgePrefab, int x, int y, EdgeOrientation orientation)
        {
            Edge edge = Instantiate(edgePrefab, transform);
            edge.transform.position = new Vector3(x * _cellSize, y * _cellSize, 0);
            edge.InitializeEdge(_blockColor, orientation);
            _edgeList.Add(edge);
        }

        void SetupCamera()
        {
            float zOffset = -10f;


            Vector3 centerPoint = new Vector3(
                _width * _cellSize / 2f,
                _height * _cellSize / 2f,
                0
            );

            Camera.main.transform.position = new Vector3(centerPoint.x, centerPoint.y, zOffset);

            float gridHeight = _height * _cellSize;
            float gridWidth = _width * _cellSize;

            float verticalSize = (gridHeight / 2f) + cameraPadding;
            float horizontalSize = (gridWidth / 2f + cameraPadding) / Camera.main.aspect;

            Camera.main.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        }
    }
}