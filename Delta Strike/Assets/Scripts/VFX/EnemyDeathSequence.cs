using System.Collections;
using Game.Core.DI;
using Game.Systems.Progress;
using Game.Audio;
using UnityEngine;

namespace Game.VFX
{
    [RequireComponent(typeof(Game.Enemies.EnemyHealth))]
    public sealed class EnemyDeathSequence : MonoBehaviour
    {
        private const int   _PointsPerKill         = 1;
        private const float _VfxAutoDestroyDelay   = 2f;
        private const float _MinShrinkScale        = 0f;
        
        [Header("FX")]
        [SerializeField] private GameObject _deathVfxPrefab;
        [SerializeField] private float _shrinkTime = 0.35f;
        [SerializeField] private AnimationCurve _shrinkCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("SFX")]
        [SerializeField] private Game.AudioClips _audioClips; 

        private Game.Enemies.EnemyHealth _hp;
        private IProgressService _progress;
        private IOneShotAudioService _audio;  

        private void Awake()
        {
            _hp = GetComponent<Game.Enemies.EnemyHealth>();
            _hp.OnDied += OnDied;
            _progress = DI.Resolve<IProgressService>();
            DI.TryResolve(out _audio);
        }

        private void OnDestroy()
        {
            if (_hp != null) _hp.OnDied -= OnDied;
        }

        private void OnDied()
        {
            _progress.AddPoints(_PointsPerKill);

            foreach (var c in GetComponents<MonoBehaviour>())
                if (c != this) c.enabled = false;

            if (_deathVfxPrefab)
            {
                var v = Instantiate(_deathVfxPrefab, transform.position, Quaternion.identity);
                Destroy(v, _VfxAutoDestroyDelay);
            }

            if (_audioClips && _audioClips.enemyDeath)
                _audio?.PlayAt(transform.position, _audioClips.enemyDeath);

            StartCoroutine(ShrinkAndDie());
        }

        private IEnumerator ShrinkAndDie()
        {
            var t0 = Time.time;
            var start = transform.localScale;
            while (Time.time - t0 < _shrinkTime)
            {
                float k = (Time.time - t0) / _shrinkTime;
                float s = Mathf.Max(_MinShrinkScale, _shrinkCurve.Evaluate(k));
                transform.localScale = start * s;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}