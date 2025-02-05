using System;
using System.Collections.Generic;
using DG.Tweening;
using Grid;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Placable
{
    public class PlaceableShapeController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private List<PlaceableShapeEdgeData> placeableShapeEdgeDatas;
        [SerializeField] private TextMeshProUGUI text;

        public void OnPointerDown(PointerEventData eventData)
        {
            print("OnPointerDown");
            Vector3 targetPosition = transform.position + Vector3.up * 150f;
            Vector3 targetScale = Vector3.one * .2f;
            transform.position = targetPosition;
            transform.localScale = targetScale;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 delta = eventData.delta;
            transform.position += new Vector3(delta.x, delta.y);
            text.SetText(Camera.main.ScreenToWorldPoint(transform.position) + "\n" +
                         Vector3Int.RoundToInt(Camera.main.ScreenToWorldPoint(transform.position)).ToString());
            GridManager.Instance.CheckPlaceArea(placeableShapeEdgeDatas);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one * .15f;
        }
    }

    [Serializable]
    public class PlaceableShapeEdgeData
    {
        public EdgeOrientation orientation;
        public Transform transform;
    }
}