using Game.Core.DI;
using Game.Core.Pooling;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Audio
{
    [DefaultExecutionOrder(-500)]
    public sealed class AudioOneShotServiceBinder : MonoBehaviour
    {
        [SerializeField] private ObjectPool _pool;
        private void Awake()
        {
            if (_pool == null)
                Debug.LogError("[AudioOneShotServiceBinder] Pool is not set.", this);
            DI.Bind<IOneShotAudioService>(new OneShotAudioService(_pool));
        }
    }
}