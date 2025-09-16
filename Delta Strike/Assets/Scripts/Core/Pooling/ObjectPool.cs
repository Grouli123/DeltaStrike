using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Pooling
{
    public sealed class ObjectPool : MonoBehaviour
    {
        [Header("Pool")]
        public GameObject prefab;
        public int initialSize = 32;
        public bool expandable = true;

        private readonly Queue<GameObject> _queue = new();

        private void Awake()
        {
            if (prefab == null)
            {
                Debug.LogError("[ObjectPool] Prefab is null.", this);
                enabled = false;
                return;
            }
            Prewarm(initialSize);
        }

        public void Prewarm(int count)
        {
            for (int i = 0; i < count; i++)
                _queue.Enqueue(CreateInstance());
        }

        private GameObject CreateInstance()
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            var po = go.GetComponent<PooledObject>();
            if (po == null) po = go.AddComponent<PooledObject>();
            po.OwnerPool = this;
            return go;
        }

        public GameObject Get()
        {
            if (_queue.Count == 0)
            {
                if (!expandable)
                    return null;
                _queue.Enqueue(CreateInstance());
            }

            var go = _queue.Dequeue();
            go.SetActive(true);
            return go;
        }

        public void Return(GameObject go)
        {
            if (go == null) return;
            go.SetActive(false);
            go.transform.SetParent(transform, false);
            _queue.Enqueue(go);
        }
    }
}