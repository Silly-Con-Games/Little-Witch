
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class EnergyTracker 
{
    public float MaxEnergy { get; internal set; }
    public float Energy { get; internal set; }

    public UnityEvent<float> onChanged;
    public UnityEvent<float> highlightEnergyCost;
    public UnityEvent stopHighlightEnergyCost;
    public UnityEvent onNotEnough;

    public EnergyTracker(float maxEnergy, float initialEnergy = 0) 
    {
        Energy = initialEnergy; 
        MaxEnergy = maxEnergy;
        onChanged = new UnityEvent<float>();
        onNotEnough = new UnityEvent();
        stopHighlightEnergyCost = new UnityEvent();
        highlightEnergyCost = new UnityEvent<float>();
    }

    public void Cleanup()
    {
        onChanged.RemoveAllListeners();
        onNotEnough.RemoveAllListeners();
    }

	public void Set(float value) {
		Energy = value;
		onChanged.Invoke(Energy);
	}

	public bool HasEnough(float amount)
    {
        if (amount > Energy)
        {
            onNotEnough.Invoke();
            return false;
        }
        return true;
    }

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

    public void HighlightCost(float cost)
    {
        highlightEnergyCost.Invoke(cost);
    }

    public void StopHighlightCost()
    {
        stopHighlightEnergyCost.Invoke();
    }

}
