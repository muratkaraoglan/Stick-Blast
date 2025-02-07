using Placable;
using UnityEngine;

namespace Pooling
{
    public class ShapePool : PoolBase
    {
        protected override GameObject Creator()
        {
            var obj = Instantiate(poolObject, transform);
            obj.GetComponent<PlaceableShapeController>().Initialize(this);
            return obj;
        }
    }
}