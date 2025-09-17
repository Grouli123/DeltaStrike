using Game.Core.Pooling;
using UnityEngine;

namespace Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(PooledObject))]
    public sealed class AudioOneShot : MonoBehaviour
    {
        private const float _PitchEpsilon     = 0.01f;   
        private const float _SpatialBlend3D   = 1f;      
        private static readonly Vector2 _NoPitchJitter = new Vector2(1f, 1f);

        [Header("Defaults")]
        public float volume = 1f;
        public Vector2 pitchJitter = _NoPitchJitter;

        private AudioSource _src;
        private PooledObject _po;

        private void Awake()
        {
            _src = GetComponent<AudioSource>();
            _po  = GetComponent<PooledObject>();
            _src.playOnAwake  = false;
            _src.spatialBlend = _SpatialBlend3D;
        }

        public void PlayAt(Vector3 pos, AudioClip clip, float? vol = null, Vector2? jitter = null)
        {
            if (clip == null) return;

            transform.position = pos;
            _src.clip    = clip;
            _src.volume  = Mathf.Clamp01(vol ?? volume);
            _src.pitch   = Random.Range((jitter ?? pitchJitter).x, (jitter ?? pitchJitter).y);

            _src.Stop();
            _src.Play();

            CancelInvoke(nameof(Return));
            Invoke(nameof(Return), clip.length / Mathf.Max(_PitchEpsilon, _src.pitch));
        }

        private void Return() => _po.ReturnToPool();
    }
}