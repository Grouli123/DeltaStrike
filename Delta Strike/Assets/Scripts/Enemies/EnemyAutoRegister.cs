using Game.Core.App;
using Game.Core.DI;
using UnityEngine;

namespace Game.Enemies
{
    [RequireComponent(typeof(EnemyHealth))]
    public sealed class EnemyAutoRegister : MonoBehaviour
    {
        private IGameStateService _s;
        private EnemyHealth _hp;

        private void Awake()
        {
            _hp = GetComponent<EnemyHealth>();
            _s  = DI.Resolve<IGameStateService>();
        }

        private void OnEnable()  => _s.RegisterEnemy(_hp);
        private void OnDisable() => _s.UnregisterEnemy(_hp);
    }

}