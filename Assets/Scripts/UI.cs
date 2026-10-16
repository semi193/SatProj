using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] Health _health;
    [SerializeField] Image _fill;
    private float _targetFill = 1f;

    private void Awake()
    {
        if (_health != null)
        {
            _targetFill = _health.Normalized;
            _fill.fillAmount = _targetFill;
        }
    }

    private void Update()
    {
        _fill.fillAmount = _health.Normalized;
    }
}
