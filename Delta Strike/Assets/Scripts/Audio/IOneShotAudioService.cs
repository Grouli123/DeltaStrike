using UnityEngine;

namespace Game.Audio
{
    public interface IOneShotAudioService
    {
        void PlayAt(Vector3 position, AudioClip clip, float volume = 1f, Vector2? pitchJitter = null);
    }
}