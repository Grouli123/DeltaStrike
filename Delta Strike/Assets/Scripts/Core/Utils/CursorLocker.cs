using Game.Core.App;
using Game.Core.DI;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class CursorLocker : MonoBehaviour
{
    [SerializeField] private bool _lockOnStart = true;
    private IGameplayBlockService _block;

    private void Awake()
    {
        _block = DI.Resolve<IGameplayBlockService>();
        _block.OnChanged += OnBlockChanged;

        if (_lockOnStart) Lock();
    }

    private void OnDestroy()
    {
        if (_block != null) _block.OnChanged -= OnBlockChanged;
    }

    private void OnBlockChanged(bool blocked)
    {
        if (blocked) Unlock();   
        else Lock();             
    }

    private void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}