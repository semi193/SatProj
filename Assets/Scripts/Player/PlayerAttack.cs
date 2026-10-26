using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] MyInputs _myInputs;
    [SerializeField] Animator _animator;
    [SerializeField] Collider _collider;
    private static readonly int AnimAttack = Animator.StringToHash("Attack");

    private void Awake()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_myInputs == null) _myInputs = GetComponent<MyInputs>();
    }

    void OnAttack()
    {
        _animator.SetTrigger(AnimAttack);
    }

    public void AttackOn()
    {
        _collider.enabled = true;
    }

    public void AttackOff()
    {
        _collider.enabled = false;
    }
}
