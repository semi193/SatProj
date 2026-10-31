using UnityEngine;

public class BossLogic : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform _player;
    [SerializeField] Animator _animator;
    [SerializeField] LayerMask _playerMask;
    [SerializeField] LayerMask _obstacleMask;
    [SerializeField] Rigidbody _rb;
    [SerializeField] Health _health;
    [SerializeField] Collider _sword;
    [SerializeField] Collider _halberd;

    [Header("Params")]
    [SerializeField] float _speed = 6f;
    [SerializeField] float _meleeDistance = 1f;
    [SerializeField] float _rotationSpeed = 720f;
    [SerializeField] float _seeDistance = 10f;
    [SerializeField] float _minWindowTime = 1f;
    [SerializeField] float _maxWindowTime = 2f;

    private static readonly int AnimWalk = Animator.StringToHash("Chase");
    private static readonly int AnimAtk1 = Animator.StringToHash("Atk1");
    private static readonly int AnimAtk2 = Animator.StringToHash("Atk2");
    private static readonly int AnimAtk3 = Animator.StringToHash("Atk3");
    private static readonly int AnimAtk4 = Animator.StringToHash("Atk4");
    private static readonly int AnimAtk5 = Animator.StringToHash("Atk5");
    private static readonly int AnimUlt = Animator.StringToHash("Ult");

    private float _windowTime = 0f;
    private float _distance => Vector3.Distance(transform.position, _player.transform.position);
    private bool _near => _distance < _meleeDistance;
    private bool _ultFlag = false;
    private bool _ultUsed = false;
    private bool _ultUsed2 = false;
    private bool canMove = true;

    private void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_health == null) _health = GetComponent<Health>();
    }
    private void FixedUpdate()
    {
        if (_windowTime > 0) _windowTime -= Time.fixedDeltaTime;

        if (!_ultUsed && _health.Normalized < 0.66)
        {
            _ultFlag = true;
            _ultUsed = true;
        }
        if (!_ultUsed2 && _health.Normalized < 0.33)
        {
            _ultFlag = true;
            _ultUsed2 = true;
        }

        Vector3 moveDir, dir = See();

        if (_near && canMove)
        {
            moveDir = Vector3.zero;
            if (_windowTime <= 0)
            {
                if (!_ultFlag)
                {
                    int _attackType = Random.Range(1, 5);
                    switch (_attackType)
                    {
                        case 1: _animator.SetTrigger(AnimAtk1); break;
                        case 2: _animator.SetTrigger(AnimAtk2); break;
                        case 3: _animator.SetTrigger(AnimAtk3); break;
                        case 4: _animator.SetTrigger(AnimAtk4); break;
                        case 5: _animator.SetTrigger(AnimAtk5); break;
                    }
                    _windowTime = Random.Range(_minWindowTime, _maxWindowTime);
                }
                else
                {
                    _animator.SetTrigger(AnimUlt);
                    _ultFlag = false;
                    _windowTime = Random.Range(_minWindowTime, _maxWindowTime);
                }
            }
        }
        else
        {
            moveDir = dir;
        }

        if (canMove)
        {
            Rotate(dir);
            Chase(moveDir);
        }
        else
        {
            _rb.linearVelocity = Vector3.zero;
        }
    }

    private void Chase(Vector3 dir)
    {
        _rb.linearVelocity = dir * _speed;
        _animator.SetBool(AnimWalk, dir != Vector3.zero);
    }

    private void Rotate(Vector3 dir)
    {
        Vector3 faceDir = dir;
        faceDir.y = 0f;
        if (faceDir.sqrMagnitude < 0.0001f) return;
        float yaw = Mathf.Atan2(faceDir.x, faceDir.z) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, yaw+90, 0f);
        Quaternion nextRot = Quaternion.RotateTowards(_rb.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(nextRot);
    }

    private Vector3 See()
    {
        Vector3 origin = gameObject.transform.position + Vector3.up;
        Vector3 dir = (_player.position - gameObject.transform.position).normalized;
        if (Physics.Raycast(origin, dir, _seeDistance, _obstacleMask) || _distance > _seeDistance) return Vector3.zero;
        else return dir;
    }

    public void SwordOn()
    {
        _sword.enabled = true;
    }

    public void SwordOff()
    {
        _sword.enabled = false;
    }

    public void HalberdOn()
    {
        _halberd.enabled = true;
    }

    public void HalberdOff()
    {
        _halberd.enabled = false;
    }

    public void MoveOn()
    {
        canMove = true;
    }

    public void MoveOff()
    {
        canMove = false;
    }
}
