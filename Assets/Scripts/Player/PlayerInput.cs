using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public Vector2 moveDir { get; private set; }
    public Vector2 lookDir { get; private set; }
    public bool runHeld { get; private set; }

    private bool _lockPressed;

    private void OnMove(InputValue v) => moveDir = v.Get<Vector2>();
    private void OnLook(InputValue v) => lookDir = v.Get<Vector2>();
    private void OnSprint(InputValue v) => runHeld = v.isPressed;
    private void OnLock(InputValue v) { if (v.isPressed) _lockPressed = true; }

    public bool ConsumeLockPressed()
    {
        bool p = _lockPressed;
        _lockPressed = false;
        return p;
    }
}