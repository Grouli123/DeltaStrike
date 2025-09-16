using Game.Core.DI;
using Game.Core.Pooling;
using UnityEngine;

namespace Game.Audio
{
    [DefaultExecutionOrder(-500)]
    public sealed class AudioOneShotServiceBinder : MonoBehaviour
    {
        [SerializeField] private ObjectPool pool;
        private void Awake()
        {
            if (pool == null)
                Debug.LogError("[AudioOneShotServiceBinder] Pool is not set.", this);
            DI.Bind<IOneShotAudioService>(new OneShotAudioService(pool));
        }
    }
}