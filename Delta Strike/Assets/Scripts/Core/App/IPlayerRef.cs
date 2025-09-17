using UnityEngine;
using Game.Player;

namespace Game.Core.App
{
    public interface IPlayerRef
    {
        Transform Transform { get; }
        PlayerController Controller { get; }
        PlayerHealth Health { get; }
        CharacterController CC { get; }

        void Attach(PlayerController pc);
    }
}