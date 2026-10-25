using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] PlayerInput _input;
    [SerializeField] CameraControl _camera;
    [SerializeField] Animator _animator;

    [Header("Movement")]
    [SerializeField] float _walkSpeed = 3f;
    [SerializeField] float _runSpeed = 6f;

    [Header("Rotation")]
    [SerializeField] float _rotationSpeed = 12f;   // градусов/сек

    private static readonly int AnimMoveX = Animator.StringToHash("MoveX");

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_input == null) _input = GetComponent<PlayerInput>();
        if (_animator == null) _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Vector2 input = _input.moveDir;

        // Направление движения относительно камеры
        Vector3 moveDir = _camera.PlanarForward * input.y + _camera.PlanarRight * input.x;
        moveDir.y = 0f;

        bool hasInput = moveDir.sqrMagnitude > 0.01f;
        bool running = hasInput && _input.runHeld;

        Move(moveDir, hasInput, running);

        Rotate(moveDir, running);
    }

    private void Move(Vector3 moveDir, bool hasInput, bool running)
    {
        float speed = running ? _runSpeed : _walkSpeed;

        Vector3 velocity = hasInput ? moveDir.normalized * speed : Vector3.zero;
        velocity.y = _rb.linearVelocity.y;   // сохраняем гравитацию / прыжок
        _rb.linearVelocity = velocity;
    }

    private void Rotate(Vector3 moveDir, bool running)
    {
        Vector3 faceDir = moveDir;

        // С локом и БЕЗ бега — стрейф: смотрим на цель, двигаемся в сторону ввода
        if (_camera.HasTarget && !running)
        {
            faceDir = _camera.Target.position - _rb.position; ;
        }

        faceDir.y = 0f;

        _animator.SetFloat(AnimMoveX, faceDir.sqrMagnitude);

        if (faceDir.sqrMagnitude < 0.0001f) return;

        float yaw = Mathf.Atan2(faceDir.x, faceDir.z) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);

        // Плавный поворот через физику
        Quaternion nextRot = Quaternion.RotateTowards(
            _rb.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime);

        _rb.MoveRotation(nextRot);
    }
}