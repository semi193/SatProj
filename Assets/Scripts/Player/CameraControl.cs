using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] Transform _player;
    [SerializeField] Camera _camera;                 // сама камера (ребЄнок пивота)
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
    [SerializeField] float _breakDistance = 30f;     // если цель дальше Ч сбрасываем
    [SerializeField] float _maxAngle = 60f;          // конус поиска
    [SerializeField] float _yawVelocity = 0f;
    [SerializeField] float _pitchVelocity = 0f;
    [SerializeField] LayerMask _targetMask;

    [Header("Collision")]
    [SerializeField] LayerMask _collisionMask;        // стены, пол, статика
    [SerializeField] float _collisionRadius = 0.25f;  // радиус Ђшарикаї камеры
    [SerializeField] float _collisionPadding = 0.05f; // зазор до стены
    [SerializeField] float _minDistance = 0.3f;       // чтобы камера не въехала в игрока

    private Vector3 _cameraLocalOffset;   // стартовое смещение камеры от пивота
    private float _currentDistance;       // текуща€ дистанци€ (дл€ сглаживани€)

    

    public Transform Target { get; private set; }
    public bool HasTarget => Target != null;

    private float _yaw;
    private float _pitch;

    // √оризонтальные оси камеры Ч используютс€ движением относительно камеры
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
        // ѕереключение лока
        if (_myInputs.ConsumeLockPressed())
            Target = HasTarget ? null : FindTarget();

        // јвтосброс, если цель убежала/умерла
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

    // ---------- ¬ращение камеры ----------

    private void FreeRotation()
    {
        Vector2 look = _myInputs.lookDir * _sensitivity;
        _yaw += look.x;
        _pitch = Mathf.Clamp(_pitch - look.y, _minPitch, _maxPitch);
    }

    private void LockedRotation()
    {
        Vector3 aimPoint = Target.position + Vector3.up;
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

    // ---------- ѕоиск цели ----------

    private Transform FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(_player.position, _maxDistance, _targetMask);
        if (hits.Length == 0) return null;

        Vector3 camFwd = Vector3.ProjectOnPlane(PlanarForward, Vector3.up).normalized;

        Transform best = null;
        float bestScore = float.MaxValue;

        foreach (var hit in hits)
        {
            Vector3 to = hit.transform.position - _player.position;
            to.y = 0f;
            if (to.sqrMagnitude < 0.001f) continue;

            float angle = Vector3.Angle(camFwd, to);
            if (angle > _maxAngle) continue;

            // „ем ближе к центру экрана и чем ближе враг Ч тем лучше
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

        // Ќаправление и точка старта Ч в мировых координатах
        Vector3 origin = transform.position;
        Vector3 worldDir = transform.TransformDirection(_cameraLocalOffset.normalized);

        float targetDistance = desired;

        if (Physics.SphereCast(origin, _collisionRadius, worldDir,
                               out RaycastHit hit, desired,
                               _collisionMask, QueryTriggerInteraction.Ignore))
        {
            // hit.distance Ч дистанци€, на которой шарик коснулс€ поверхности
            targetDistance = Mathf.Max(hit.distance - _collisionPadding, _minDistance);
        }

        // —глаживание: быстро прижимаемс€ к стене, плавно отъезжаем обратно
        if (targetDistance < _currentDistance)
            _currentDistance = targetDistance;                          // мгновенно прижались
        else
            _currentDistance = Mathf.SmoothDamp(_currentDistance, targetDistance, ref _distanceVelocity, _smoothTime);

        // —тавим камеру в мировую позицию Ч Unity сам пересчитает localPosition
        _camera.transform.position = origin + worldDir * _currentDistance;
    }
}