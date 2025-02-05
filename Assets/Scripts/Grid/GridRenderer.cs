using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridRenderer : MonoBehaviour
    {
        private int _width = 8;
        private int _height = 8;
        private float _cellSize = 1f;
        private float _lineWidth = 0.1f;
        private Color _lineColor = Color.white;
        [SerializeField] private Material gridMaterial;


        public void InitializeGridRenderer(int width, int height, float cellSize, float lineWidth, Color gridColor)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _lineWidth = lineWidth;
            _lineColor = gridColor;
            CreateGrid();
            SetupCamera();
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
            for (int y = 0; y <= _width; y++)
            {
                vertices.Add(new Vector3(0, y * _cellSize - _lineWidth / 2, 0)); // sol alt
                vertices.Add(new Vector3(_width * _cellSize, y * _cellSize - _lineWidth / 2, 0)); // sağ alt
                vertices.Add(new Vector3(0, y * _cellSize + _lineWidth / 2, 0)); // sol üst
                vertices.Add(new Vector3(_width * _cellSize, y * _cellSize + _lineWidth / 2, 0)); // sağ üst

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

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();

            var meshFilter = gameObject.AddComponent<MeshFilter>();
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshFilter.mesh = mesh;
            gridMaterial.SetColor("_Color", _lineColor);
            meshRenderer.material = gridMaterial;
            gameObject.isStatic = true;
        }

        void SetupCamera()
        {
            float zOffset = -10f;
            float padding = 1f;

            Vector3 centerPoint = new Vector3(
                _width * _cellSize / 2f,
                _height * _cellSize / 2f,
                0
            );

            Camera.main.transform.position = new Vector3(centerPoint.x, centerPoint.y, zOffset);

            float gridHeight = _height * _cellSize;
            float gridWidth = _width * _cellSize;

            float verticalSize = (gridHeight / 2f) + padding;
            float horizontalSize = (gridWidth / 2f + padding) / Camera.main.aspect;

            Camera.main.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        }
    }
}