
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

    public void Cleanup()
    {
        onChanged.RemoveAllListeners();
    }

    public void Set(float value) {
        if (Health == value)
            return;
        Health = value;
		onChanged.Invoke(Health);
	}

    public bool IsDepleted => Health <= 0;

    public void ResetHealth() 
    {
        if (Health == MaxHealth)
            return;
        Health = MaxHealth;
        onChanged.Invoke(Health);
    }

    public void TakeDamage(float amount) 
    {
        Assert.IsFalse(amount < 0);
        if (Health == 0)
            return;
        Health = Mathf.Max(0, Health - amount);
        onChanged.Invoke(Health);

    }
    public void Heal(float amount) 
    {
        Assert.IsFalse(amount < 0);
        if (Health == MaxHealth)
            return;
        Health = Mathf.Min(MaxHealth, Health + amount);
        onChanged.Invoke(Health);
    }
}