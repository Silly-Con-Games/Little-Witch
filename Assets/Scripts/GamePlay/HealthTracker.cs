
using UnityEngine;

public class HealthTracker
{
    private float maxHealth = 0;
    public float Health { get; internal set; }
    public HealthTracker(float maxHealth) => Health = this.maxHealth = maxHealth;
    public bool IsDepleted => Health <= 0;
    public void ResetHealth() => Health = maxHealth;
    public void TakeDamage(float amount) => Health = Mathf.Max(0, Health - amount);
    public void Heal(float amount) => Health = Mathf.Min(maxHealth, Health + amount);
}