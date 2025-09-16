using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/AudioClips", fileName = "AudioClips")]
    public sealed class AudioClips : ScriptableObject
    {
        public AudioClip fire;
        public AudioClip impact;
        public AudioClip enemyDeath;
        public AudioClip playerHit;
    }
}