using System;
using System.Collections.Generic;
using DG.Tweening;
using Grid;
using Placeable;
using Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Placable
{
    public class PlaceableShapeController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private List<PlaceableShapeEdgeData> placeableShapeEdgeDatas;
        private bool _canPlace;
        private readonly float _targetY = 150f;
        private readonly float _defaultScale = .2f;
        private readonly float _targetScale = .25f;
        private ShapePool _shapePool;

        private bool _isSelected;

        public void Initialize(ShapePool shapePool)
        {
            _shapePool = shapePool;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if( ShapesUIManager.Instance.isShapeSelected) return;
            ShapesUIManager.Instance.isShapeSelected = true;
            Vector3 targetPosition = transform.position + Vector3.up * _targetY;
            Vector3 targetScale = Vector3.one * _targetScale;
            transform.position = targetPosition;
            transform.localScale = targetScale;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.delta;
            transform.position += new Vector3(delta.x, delta.y);
            _canPlace = GridManager.Instance.CanPlaceShape(placeableShapeEdgeDatas);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_canPlace)
            {
                GridManager.Instance.PlaceShape();
                ShapesUIManager.Instance.OnShapeRelease(_shapePool, gameObject);
                _canPlace = false;
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }

            transform.localScale = Vector3.one * _defaultScale;
            ShapesUIManager.Instance.isShapeSelected = false;
        }
    }

    [Serializable]
    public class PlaceableShapeEdgeData
    {
        public EdgeOrientation orientation;
        public Transform transform;
    }
}