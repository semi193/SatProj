using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private HealthEventChannel _healthEventChannel;
    [SerializeField] private Image _healthBarFill;

    [Header("Настройки SmoothDamp")]
    [SerializeField] private float _smoothTime = 0.2f;

    private float _targetFillAmount = 1f;
    private float _currentFillAmount;
    private float _velocity = 0f;

    private void OnEnable()
    {
        if (_healthEventChannel != null)
        {
            _healthEventChannel.OnHealthChanged += UpdateTargetHealth;
            _healthEventChannel.OnDied += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (_healthEventChannel != null)
        {
            _healthEventChannel.OnHealthChanged -= UpdateTargetHealth;
            _healthEventChannel.OnDied -= HandleDeath;
        }
    }

    private void Start()
    {
        if (_healthBarFill != null)
        {
            _currentFillAmount = _healthBarFill.fillAmount;
            _targetFillAmount = _currentFillAmount;
        }
    }

    private void UpdateTargetHealth(float currentHealth, float maxHealth)
    {
        _targetFillAmount = currentHealth / maxHealth;
    }

    private void Update()
    {
        if (_healthBarFill == null) return;
        if (Mathf.Approximately(_currentFillAmount, _targetFillAmount)) return;

        _currentFillAmount = Mathf.SmoothDamp(_currentFillAmount, _targetFillAmount, ref _velocity, _smoothTime);
        _healthBarFill.fillAmount = _currentFillAmount;
    }

    private void HandleDeath()
    {
        Debug.Log("UI: Игрок умер.");
    }
}