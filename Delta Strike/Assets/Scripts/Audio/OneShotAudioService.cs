using Game.Core.Pooling;
using UnityEngine;

namespace Game.Audio
{
    public sealed class OneShotAudioService : IOneShotAudioService
    {
        private readonly ObjectPool _pool;
        public OneShotAudioService(ObjectPool pool) => _pool = pool;

        public void PlayAt(Vector3 position, AudioClip clip, float volume = 1f, Vector2? pitchJitter = null)
        {
            if (_pool == null || clip == null) return;
            var go = _pool.Get();
            var one = go.GetComponent<AudioOneShot>();
            if (one != null) one.PlayAt(position, clip, volume, pitchJitter ?? new Vector2(1f, 1f));
        }
    }
}