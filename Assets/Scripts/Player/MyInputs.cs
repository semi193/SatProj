using UnityEngine;
using UnityEngine.InputSystem;

public class MyInputs : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private VoidEventChannel _pauseToggledEvent;

    [SerializeField] private float _gamepadSensitivity = 2.5f;

    public bool canMove = true;
    public bool canInput = true;

    public bool _isPaused = false; 

    private Vector2 _rawInputDir;
    private bool _rawRunHeld;

    public Vector2 inputDir => (_isPaused) ? Vector2.zero : _rawInputDir;
    public bool runHeld => !(_isPaused) && _rawRunHeld;

    public Vector2 lookDir { get; private set; }
    private bool _lockPressed;

    private void OnEnable()
    {
        if (_pauseToggledEvent != null)
            _pauseToggledEvent.OnEventRaised += HandlePauseState;
    }

    private void OnDisable()
    {
        if (_pauseToggledEvent != null)
            _pauseToggledEvent.OnEventRaised -= HandlePauseState;
    }

    private void HandlePauseState()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            lookDir = Vector2.zero;
        }
    }


    private void OnLook(InputValue v)
    {
        if (_isPaused) return;

        Vector2 rawInput = v.Get<Vector2>();
        bool isGamepadInput = false;

        if (Gamepad.current != null)
        {
            Vector2 stickValue = Gamepad.current.rightStick.ReadValue();
            if (Vector2.Distance(rawInput, stickValue) < 0.1f)
            {
                isGamepadInput = true;
            }
        }

        lookDir = isGamepadInput ? rawInput * _gamepadSensitivity : rawInput;
    }

    private void OnLock(InputValue v)
    {
        if (_isPaused) return;
        if (v.isPressed) _lockPressed = true;
    }

    public bool ConsumeLockPressed()
    {
        bool p = _lockPressed;
        _lockPressed = false;
        return p;
    }

    private void OnMove(InputValue v) => _rawInputDir = v.Get<Vector2>();
    private void OnSprint(InputValue v) => _rawRunHeld = v.isPressed;

    private void OnPause(InputValue v)
    {
        if (v.isPressed)
        {
            _pauseToggledEvent.RaiseEvent();
        }
    }

    public void MoveOn()
    {
        canMove = true;
        canInput = true;
    }
    public void MoveOff()
    {
        canMove = false;
        canInput = false;
    }
    public void InputOn()
    {
        canInput = true;
    }
}