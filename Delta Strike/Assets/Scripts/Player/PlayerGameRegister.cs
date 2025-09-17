using Game.Core.App;
using Game.Core.DI;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(PlayerHealth))]
    [DefaultExecutionOrder(200)]
    public sealed class PlayerGameRegister : MonoBehaviour
    {
        private void Start()
        {
            if (DI.TryResolve<IGameStateService>(out var state))
                state.RegisterPlayer(GetComponent<PlayerHealth>());
            else
                Debug.LogError("[PlayerGameRegister] IGameStateService not bound");
        }
    }
}