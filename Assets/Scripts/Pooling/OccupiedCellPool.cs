using DG.Tweening;
using UnityEngine;

namespace Pooling
{
    public class OccupiedCellPool : PoolBase
    {
        protected override void OnObjectReleased(GameObject obj)
        {
            obj.transform.DOScale(0, .1f).OnComplete(() =>
            {
                obj.SetActive(false);
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.zero;
            });
        }
    }
}