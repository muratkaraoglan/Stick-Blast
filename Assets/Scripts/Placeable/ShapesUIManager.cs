using System;
using System.Collections.Generic;
using Placable;
using Pooling;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

namespace Placeable
{
    public class ShapesUIManager : Singleton<ShapesUIManager>
    {
        [SerializeField] private List<ShapePool> shapePools;
        [SerializeField] private Transform[] slots = new Transform[3];
        private int _activeShapeCount;
        [HideInInspector] public bool isShapeSelected;

        private void Start()
        {
            Input.multiTouchEnabled = false;
            FillSlots();
        }

        private void FillSlots()
        {
            for (int i = 0; i < 3; i++)
            {
                var shapePool = shapePools[Random.Range(0, shapePools.Count)];
                var shape = shapePool.Pool.Get();
                shape.transform.SetParent(slots[i]);
                shape.transform.localPosition = Vector3.zero;
            }

            _activeShapeCount = 3;
        }

        public void OnShapeRelease(ShapePool shapePool, GameObject shape)
        {
            shapePool.Pool.Release(shape);
            _activeShapeCount--;
            if (_activeShapeCount == 0) FillSlots();
        }
    }
}