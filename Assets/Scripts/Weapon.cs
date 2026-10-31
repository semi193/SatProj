using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float _damage = 10f;
    [SerializeField] LayerMask _target;
    private Health _health;
    int _targetIndex => Mathf.RoundToInt(Mathf.Log(_target.value, 2));

    private void OnTriggerEnter(Collider other)
    {
        if (other.GameObject().layer == _targetIndex)
        {
            other.GameObject().TryGetComponent<Health>(out _health);
            _health.GetDamage(_damage);
        }
    }
}
