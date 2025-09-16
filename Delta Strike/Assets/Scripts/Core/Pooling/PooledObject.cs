using UnityEngine;

namespace Game.Core.Pooling
{
    public sealed class PooledObject : MonoBehaviour
    {
        public ObjectPool OwnerPool { get; set; }

        public void ReturnToPool()
        {
            if (OwnerPool != null)
                OwnerPool.Return(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}