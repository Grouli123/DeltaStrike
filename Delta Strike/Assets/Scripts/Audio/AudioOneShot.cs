using Game.Core.Pooling;
using UnityEngine;

namespace Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PooledObject))]
    public sealed class AudioOneShot : MonoBehaviour
    {
        [Header("Defaults")]
        public float volume = 1f;
        public Vector2 pitchJitter = new Vector2(1f, 1f);

        private AudioSource _src;
        private PooledObject _po;

        private void Awake()
        {
            _src = GetComponent<AudioSource>();
            _po  = GetComponent<PooledObject>();
            _src.playOnAwake = false;
            _src.spatialBlend = 1f; 
        }

        public void PlayAt(Vector3 pos, AudioClip clip, float? vol = null, Vector2? jitter = null)
        {
            if (clip == null) return;
            transform.position = pos;
            _src.clip = clip;

            var v = Mathf.Clamp01(vol ?? volume);
            var j = jitter ?? pitchJitter;
            _src.volume = v;
            _src.pitch  = Random.Range(j.x, j.y);

            _src.Stop();
            _src.Play();
            CancelInvoke(nameof(Return));
            Invoke(nameof(Return), clip.length / Mathf.Max(0.01f, _src.pitch));
        }

        private void Return() => _po.ReturnToPool();
    }
}