using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private Animator _animator;
    [SerializeField] SoundManager _soundManager;
    [SerializeField] private AudioClip _damageSound;

    public float Normalized => health / maxHealth;

    [SerializeField] private HealthEventChannel _healthEventChannel;

    [Header("Game End Events")]
    [SerializeField] private VoidEventChannel _defeatEvent;

    private static readonly int AnimGetDmg = Animator.StringToHash("GetDmg");
    private static readonly int AnimDead = Animator.StringToHash("Dead");

    private void Awake()
    {
        health = maxHealth;
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_soundManager == null) _soundManager = GetComponent<SoundManager>();
    }

    private void Start()
    {
        if (_healthEventChannel != null)
            _healthEventChannel.RaiseHealthChanged(health, maxHealth);
    }

    public void GetDamage(float dmg)
    {
        _soundManager.PlaySound(_damageSound);
        health -= dmg;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (_animator != null) _animator.SetTrigger(AnimGetDmg);

        if (_healthEventChannel != null)
            _healthEventChannel.RaiseHealthChanged(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (_animator != null) _animator.SetBool(AnimDead, true);

        if (_healthEventChannel != null)
            _healthEventChannel.RaiseDied();

        if (_defeatEvent != null)
        {
            _defeatEvent.RaiseEvent();
        }
    }
}