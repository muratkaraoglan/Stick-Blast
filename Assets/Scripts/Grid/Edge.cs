using UnityEngine;

namespace Grid
{
    public class Edge : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        private EdgeOrientation _edgeOrientation;
        private bool _edgeIsOccupied;
        private Vector3 _edgePosition;
        private Vector3Int _edgePositionInt;

        public void InitializeEdge(Color color, EdgeOrientation edgeOrientation)
        {
            spriteRenderer.color = color;
            _edgeOrientation = edgeOrientation;
            _edgePosition = spriteRenderer.transform.position;
            _edgePositionInt = Vector3Int.FloorToInt(_edgePosition);
        }

        public Vector3 GetEdgePosition() => _edgePosition;
        public Vector3Int GetEdgePositionInt() => _edgePositionInt;

        public bool IsEdgeOccupied() => _edgeIsOccupied;
        public void SetEdgeIsOccupied(bool value) => _edgeIsOccupied = value;
        public EdgeOrientation Orientation => _edgeOrientation;
    }
}