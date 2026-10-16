using System;
using System.Xml.Serialization;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float maxHealth = 10f;
    public float Normalized => health / maxHealth;

    private void Awake()
    {
        health = maxHealth;
    }

    public void GetDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        print("Dead");
    }
}
