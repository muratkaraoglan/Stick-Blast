using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public abstract class PoolBase : MonoBehaviour
    {
        [SerializeField] protected GameObject poolObject;
        [SerializeField] protected int maxCount = 20;
        public IObjectPool<GameObject> Pool;

        protected virtual void Awake()
        {
            var list = new List<GameObject>();
            for (var i = 0; i < maxCount; i++)
            {
                poolObject.SetActive(false);
                list.Add(Creator());
            }

            Pool = new ObjectPooling<GameObject>(Creator, OnObjectAcquired, OnObjectReleased, list);
        }

        protected virtual GameObject Creator()
        {
            var obj = Instantiate(poolObject, transform);
            return obj;
        }

        protected virtual void OnObjectAcquired(GameObject obj)
        {
            obj.SetActive(true);

            // other logics ...
        }

        protected virtual void OnObjectReleased(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            // other logics ...
        }
    }
}