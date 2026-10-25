using UnityEngine;
using UnityEngine.InputSystem;

public class MyInputs : MonoBehaviour
{
    public bool canMove = true;

    // camera
    public Vector2 lookDir { get; private set; }
    private bool _lockPressed;
    private void OnLook(InputValue v) => lookDir = v.Get<Vector2>();
    private void OnLock(InputValue v) { if (v.isPressed) _lockPressed = true; }
    public bool ConsumeLockPressed()
    {
        bool p = _lockPressed;
        _lockPressed = false;
        return p;
    }

    //movement
    public Vector2 inputDir;
    public bool runHeld;
    private void OnMove(InputValue v) => inputDir = v.Get<Vector2>();
    private void OnSprint(InputValue v) => runHeld = v.isPressed;
}
