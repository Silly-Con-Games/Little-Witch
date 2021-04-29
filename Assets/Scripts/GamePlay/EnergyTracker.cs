
using UnityEngine;
using UnityEngine.Assertions;

public class EnergyTracker 
{
    private float maxEnergy = 0;
    public float Energy { get; internal set; }
    public EnergyTracker(float maxEnergy, float initialEnergy = 0) 
    {
        Energy = initialEnergy; 
        this.maxEnergy = maxEnergy;     
    }

    public bool HasEnough(float amount) => amount <= Energy;

    public bool CanFitMore => Energy < maxEnergy;
    public void UseEnergy(float amount) 
    {
        Energy -= amount;
        Assert.IsTrue(Energy < 0, "Oi oi not enough energy for that, what are you doin");
    }
    public void AddEnergy(float amount) => Energy = Mathf.Min(maxEnergy, Energy + amount);

}
