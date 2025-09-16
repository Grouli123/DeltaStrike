using System.Collections;
using UnityEngine;

namespace Game.VFX
{
    public sealed class MuzzleFlash : MonoBehaviour
    {
        [SerializeField] private GameObject flashObject;   
        [SerializeField] private Light flashLight;         
        [SerializeField] private float flashTime = 0.05f;

        private Coroutine _co;

        private void Awake()
        {
            if (flashObject) flashObject.SetActive(false);
            if (flashLight)  flashLight.enabled = false;
        }

        public void Play()
        {
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            if (flashObject) flashObject.SetActive(true);
            if (flashLight)  flashLight.enabled = true;

            yield return new WaitForSeconds(flashTime);

            if (flashObject) flashObject.SetActive(false);
            if (flashLight)  flashLight.enabled = false;
            _co = null;
        }
    }
}