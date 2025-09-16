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
        [Header("FX")]
        [SerializeField] private GameObject deathVfxPrefab;
        [SerializeField] private float shrinkTime = 0.35f;
        [SerializeField] private AnimationCurve shrinkCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("SFX")]
        [SerializeField] private Game.AudioClips audioClips; 

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
            _progress.AddPoints(1);

            foreach (var c in GetComponents<MonoBehaviour>())
                if (c != this) c.enabled = false;

            if (deathVfxPrefab)
            {
                var v = Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
                Destroy(v, 2f);
            }

            if (audioClips && audioClips.enemyDeath)
                _audio?.PlayAt(transform.position, audioClips.enemyDeath);

            StartCoroutine(ShrinkAndDie());
        }

        private IEnumerator ShrinkAndDie()
        {
            var t0 = Time.time;
            var start = transform.localScale;
            while (Time.time - t0 < shrinkTime)
            {
                float k = (Time.time - t0) / shrinkTime;
                float s = Mathf.Max(0f, shrinkCurve.Evaluate(k));
                transform.localScale = start * s;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}