using UnityEngine;

namespace Game.Enemies
{
    [CreateAssetMenu(menuName = "Game/EnemyConfig", fileName = "EnemyConfig")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [Tooltip("Enemies spawn with HP in [minHP, maxHP] range.")]
        public float minHP = 40f;
        public float maxHP = 120f;

        [Header("Simple AI")]
        public float moveSpeed = 2f;
        public float detectRange = 20f;
        public float stopDistance = 1.5f;
    }
}