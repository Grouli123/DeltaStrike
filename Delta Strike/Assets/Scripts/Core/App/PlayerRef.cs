using UnityEngine;

namespace Game.Core.App
{
    public interface IPlayerRef
    {
        Transform Transform { get; }
        Game.Player.PlayerController Controller { get; }
        Game.Player.PlayerHealth Health { get; }
        CharacterController CC { get; }

        void Attach(Game.Player.PlayerController pc);
    }

    public sealed class PlayerRef : IPlayerRef
    {
        public Transform Transform { get; private set; }
        public Game.Player.PlayerController Controller { get; private set; }
        public Game.Player.PlayerHealth Health { get; private set; }
        public CharacterController CC { get; private set; }

        public void Attach(Game.Player.PlayerController pc)
        {
            Controller = pc;
            Transform = pc.transform;
            CC = pc.GetComponent<CharacterController>();
            Health = pc.GetComponentInChildren<Game.Player.PlayerHealth>() ?? pc.GetComponent<Game.Player.PlayerHealth>();
        }
    }
}