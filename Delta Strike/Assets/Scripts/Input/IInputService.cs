using UnityEngine;

namespace Game.Input
{
    public enum InputMode { Desktop, Mobile }

    public interface IInputService
    {
        Vector2 MoveAxis { get; }          
        Vector2 LookDelta { get; }        
        bool IsFirePressed { get; }        
        bool IsOpenUpgradePressed { get; } 
    }
}