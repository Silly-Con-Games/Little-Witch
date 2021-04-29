using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    private Slider healthbar;

    [SerializeField]
    private Slider energybar;

    public void SetUpHealth(float startingHealth, float maxHealth)
    {
        healthbar.minValue = 0;
        healthbar.maxValue = maxHealth;
        healthbar.value = startingHealth;
    }
    public void SetUpEnergy(float startingEnergy, float maxEnergy)
    {
        energybar.minValue = 0;
        energybar.maxValue = maxEnergy;
        energybar.value = startingEnergy;
    }

    public void SetHealth(float newHealth) => healthbar.value = newHealth;

    public void SetEnergy(float newEnergy) => energybar.value = newEnergy;
}
