
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class EnergyTracker 
{
    public float MaxEnergy { get; internal set; }
    public float Energy { get; internal set; }

    public UnityEvent<float> onChanged;

    public EnergyTracker(float maxEnergy, float initialEnergy = 0) 
    {
        Energy = initialEnergy; 
        MaxEnergy = maxEnergy;
        onChanged = new UnityEvent<float>();
    }

    public bool HasEnough(float amount) => amount <= Energy;

    public bool CanFitMore => Energy < MaxEnergy;

    public void UseEnergy(float amount) 
    {
        Assert.IsFalse(amount < 0);

        Energy -= amount;

        Assert.IsFalse(Energy < 0);

        onChanged.Invoke(Energy);
    }

    public void AddEnergy(float amount)
    {
        Assert.IsFalse(amount < 0);

        Energy = Mathf.Min(MaxEnergy, Energy + amount);

        onChanged.Invoke(Energy);
    }

}
