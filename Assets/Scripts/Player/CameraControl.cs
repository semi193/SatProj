using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform _player;
    [SerializeField] Camera _camera;
    [SerializeField] MyInputs _myInputs;

    [Header("Look")]
    [SerializeField] float _sensitivity = 0.15f;
    [SerializeField] float _offset = 3f;
    [SerializeField] float _minPitch = -60f;
    [SerializeField] float _maxPitch = 60f;
    [SerializeField] float _distanceVelocity = 0f;
    [SerializeField] float _smoothTime = 0.2f;

    [Header("Lock-On")]
    [SerializeField] float _maxDistance = 20f;
    [SerializeField] float _breakDistance = 30f;
    [SerializeField] float _maxAngle = 60f;
    [SerializeField] float _yawVelocity = 0f;
    [SerializeField] float _pitchVelocity = 0f;
    [SerializeField] LayerMask _targetMask;

    [Header("Collision")]
    [SerializeField] LayerMask _collisionMask;
    [SerializeField] float _collisionRadius = 0.25f;
    [SerializeField] float _collisionPadding = 0.05f;
    [SerializeField] float _minDistance = 0.3f;

    private Vector3 _cameraLocalOffset;
    private float _currentDistance;

    

    public Transform Target { get; private set; }
    public bool HasTarget => Target != null;

    private float _yaw;
    private float _pitch;

    public Vector3 PlanarForward => Quaternion.Euler(0f, _yaw, 0f) * Vector3.forward;
    public Vector3 PlanarRight => Quaternion.Euler(0f, _yaw, 0f) * Vector3.right;

    private void Awake()
    {
        if (_camera == null) _camera = GetComponentInChildren<Camera>();

        _cameraLocalOffset = _camera.transform.localPosition;
        _currentDistance = _cameraLocalOffset.magnitude;

        _yaw = _player.eulerAngles.y;
        _pivotSnap();
    }

    private void Update()
    {
        if (_myInputs.ConsumeLockPressed())
            Target = HasTarget ? null : FindTarget();

        if (HasTarget)
        {
            float dist = Vector3.Distance(_player.position, Target.position);
            if (Target == null || dist > _breakDistance)
                Target = null;
        }
    }

    private void LateUpdate()
    {
        if (HasTarget) LockedRotation();
        else FreeRotation();

        _pivotSnap();
        UpdateCameraCollision();
    }


    private void FreeRotation()
    {
        Vector2 look = _myInputs.lookDir * _sensitivity;
        _yaw += look.x;
        _pitch = Mathf.Clamp(_pitch - look.y, _minPitch, _maxPitch);
    }

    private void LockedRotation()
    {
        Vector3 aimPoint = Target.position;
        Vector3 dir = (aimPoint - _camera.transform.position).normalized;

        float targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float targetPitch = -Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        targetPitch = Mathf.Clamp(targetPitch, _minPitch, _maxPitch);

        _yaw = Mathf.SmoothDampAngle(_yaw, targetYaw, ref _yawVelocity, _smoothTime);
        _pitch = Mathf.SmoothDampAngle(_pitch, targetPitch, ref _pitchVelocity, _smoothTime);
    }

    private void _pivotSnap()
    {
        transform.position = _player.position + new Vector3(0, _offset, 0);
        transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }


    private Transform FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(_player.position, _maxDistance, _targetMask);
        if (hits.Length == 0) return null;

        Vector3 camFwd = Vector3.ProjectOnPlane(PlanarForward, Vector3.up).normalized;

        Transform best = null;
        float bestScore = float.MaxValue;

        foreach (var hit in hits)
        {
            Vector3 to = hit.transform.position - _camera.transform.position;
            to.y = 0f;
            if (to.sqrMagnitude < 0.001f) continue;

            float angle = Vector3.Angle(camFwd, to);
            if (angle > _maxAngle) continue;

            float score = angle + to.magnitude * 0.5f;
            if (score < bestScore)
            {
                bestScore = score;
                best = hit.transform;
            }
        }
        return best;
    }

    private void UpdateCameraCollision()
    {
        float desired = _cameraLocalOffset.magnitude;
        if (desired < 0.001f) return;

        Vector3 origin = transform.position;
        Vector3 worldDir = transform.TransformDirection(_cameraLocalOffset.normalized);

        float targetDistance = desired;

        if (Physics.SphereCast(origin, _collisionRadius, worldDir,
                               out RaycastHit hit, desired,
                               _collisionMask, QueryTriggerInteraction.Ignore))
        {
            targetDistance = Mathf.Max(hit.distance - _collisionPadding, _minDistance);
        }

        if (targetDistance < _currentDistance)
            _currentDistance = targetDistance;
        else
            _currentDistance = Mathf.SmoothDamp(_currentDistance, targetDistance, ref _distanceVelocity, _smoothTime);

        _camera.transform.position = origin + worldDir * _currentDistance;
    }
}