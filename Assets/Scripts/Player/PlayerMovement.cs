using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerMovement : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] CameraControl _camera;
    [SerializeField] Animator _animator;
    [SerializeField] MyInputs _myInputs;

    [Header("Movement")]
    [SerializeField] float _walkSpeed = 3f;
    [SerializeField] float _runSpeed = 6f;

    [Header("Dash")]
    [SerializeField] float _dashForce = 12f;
    [SerializeField] LayerMask _normalLayer;
    [SerializeField] LayerMask _invisibleLayer;

    [Header("Rotation")]
    [SerializeField] float _rotationSpeed = 720f;

    private static readonly int AnimDash = Animator.StringToHash("Dash");
    private static readonly int AnimMoveX = Animator.StringToHash("MoveX");
    private static readonly int AnimMoveSpeed = Animator.StringToHash("MoveSpeed");

    Vector2 input => _myInputs.inputDir;
    Vector3 moveDir => _camera.PlanarForward * input.y + _camera.PlanarRight * input.x;
    bool hasInput => moveDir.sqrMagnitude > 0.01f;
    bool running => hasInput && _myInputs.runHeld;
    bool dashing = false;

    int _invisibleLayerIndex => Mathf.RoundToInt(Mathf.Log(_invisibleLayer.value, 2));
    int _normalLayerIndex => Mathf.RoundToInt(Mathf.Log(_normalLayer.value, 2));

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_myInputs == null) _myInputs = GetComponent<MyInputs>();
    }

    private void FixedUpdate()
    {
        if (_myInputs.canMove)
        {
            Rotate();
            if (!dashing) Move();
        }
        else
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
        }
    }

    private void OnDash()
    {
        if (_myInputs.canInput && !_myInputs._isPaused) _animator.SetTrigger(AnimDash);
    }
    private void Dash()
    {
        dashing = true;
        gameObject.layer = _invisibleLayerIndex;
        Vector3 velocity = hasInput ? moveDir.normalized * _dashForce : -_rb.transform.forward * _dashForce;
        _rb.linearVelocity = velocity;
    }
    private void DashOff()
    {
        dashing = false;
        gameObject.layer = _normalLayerIndex;
    }

    private void Move()
    {
        float speed = running ? _runSpeed : _walkSpeed;

        Vector3 velocity = hasInput ? moveDir.normalized * speed : Vector3.zero;
        _rb.linearVelocity = velocity;

        if (_camera.HasTarget && !running && input.y < 0)
        {
            _animator.SetFloat(AnimMoveSpeed, -1);
        }
        else
        {
            _animator.SetFloat(AnimMoveSpeed, speed / _walkSpeed);
        }

        _animator.SetFloat(AnimMoveX, hasInput ? 1 : 0);
    }

    private void Rotate()
    {
        Vector3 faceDir = moveDir;

        if (_camera.HasTarget && !running)
        {
            faceDir = _camera.Target.position - _rb.position;
        }

        faceDir.y = 0f;

        if (faceDir.sqrMagnitude < 0.0001f) return;

        float yaw = Mathf.Atan2(faceDir.x, faceDir.z) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);

        Quaternion nextRot = Quaternion.RotateTowards(_rb.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime);

        _rb.MoveRotation(nextRot);
    }
}