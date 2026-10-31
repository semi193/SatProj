using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthEventChannel", menuName = "Events/Health Event Channel")]
public class HealthEventChannel : ScriptableObject
{
    public event Action<float, float> OnHealthChanged;

    public event Action OnDied;

    public void RaiseHealthChanged(float current, float max)
    {
        OnHealthChanged?.Invoke(current, max);
    }

    public void RaiseDied()
    {
        OnDied?.Invoke();
    }
}