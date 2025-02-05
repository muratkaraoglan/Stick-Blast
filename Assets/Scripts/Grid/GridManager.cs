using System;
using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GridRenderer gridRenderer;

        [Header("Grid Settings")] [SerializeField]
        private int width = 6;

        [SerializeField] private int height = 6;
        [SerializeField] private float cellSize = 2.5f;
        [SerializeField] private float lineWidth = 0.3f;
        [SerializeField] private Color gridColor = Color.white;

        private void Start()
        {
            gridRenderer.InitializeGridRenderer(width, height, cellSize, lineWidth, gridColor);
        }
    }
}