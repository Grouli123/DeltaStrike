using System.Collections;
using UnityEngine;

namespace Game.VFX
{
    public sealed class MuzzleFlash : MonoBehaviour
    {
        [SerializeField] private GameObject _flashObject;
        [SerializeField] private Light _flashLight;  
        [SerializeField] private float _flashTime = 0.05f;

        private Coroutine _co;

        private void Awake()
        {
            if (_flashObject) _flashObject.SetActive(false);
            if (_flashLight)  _flashLight.enabled = false;
        }

        public void Play()
        {
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            if (_flashObject) _flashObject.SetActive(true);
            if (_flashLight)  _flashLight.enabled = true;

            yield return new WaitForSeconds(_flashTime);

            if (_flashObject) _flashObject.SetActive(false);
            if (_flashLight)  _flashLight.enabled = false;
            _co = null;
        }
    }
}