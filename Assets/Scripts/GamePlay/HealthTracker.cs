
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class HealthTracker
{
    public float MaxHealth { get; internal set; }

    public UnityEvent<float> onChanged;

    public float Health { get; internal set; }

    public HealthTracker(float maxHealth)
    { 
        Health = this.MaxHealth = maxHealth;
        onChanged = new UnityEvent<float>();
    }

	public void Set(float value) {
		Health = value;
		onChanged.Invoke(Health);
	}

    public bool IsDepleted => Health <= 0;

    public void ResetHealth() 
    {
        Health = MaxHealth;
        onChanged.Invoke(Health);
    }

    public void TakeDamage(float amount) 
    {
        Assert.IsFalse(amount < 0);

        Health = Mathf.Max(0, Health - amount);
        onChanged.Invoke(Health);

    }
    public void Heal(float amount) 
    {
        Assert.IsFalse(amount < 0);

        Health = Mathf.Min(MaxHealth, Health + amount);
        onChanged.Invoke(Health);
    }
}