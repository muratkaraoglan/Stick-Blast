using UnityEngine;

namespace Pooling
{
    public class OccupiedCellPool : PoolBase
    {
        protected override void OnObjectReleased(GameObject obj)
        {
            base.OnObjectReleased(obj);
            obj.transform.localScale = Vector3.zero;
        }
    }
}