using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // health and energy
    [SerializeField] private Slider[] healthbar;
    private float oldHealth;
    [SerializeField] private Slider[] energybar;
    private float oldEnergy;

    // ability icons
    [SerializeField] private Image[] icons;
    private Image[] iconsInner;
    private Color[] iconColors;

    private void Start()
    {
        // ability icons
        iconsInner = new Image[icons.Length];
        iconColors = new Color[icons.Length];
        for (int i = 0; i < icons.Length; i++)
        {
            iconsInner[i] = icons[i].GetComponentsInChildren<Image>()[1];
            iconColors[i] = iconsInner[i].color;
        }
    }

    public void SetUpHealth(float startingHealth, float maxHealth)
    {
        SetUpBar(startingHealth, maxHealth, ref healthbar);
        oldHealth = startingHealth;
    }

    public void SetUpEnergy(float startingEnergy, float maxEnergy)
    {
        SetUpBar(startingEnergy, maxEnergy, ref energybar);
        oldEnergy = startingEnergy;
    }

    private void SetUpBar(float startingVal, float maxVal, ref Slider[] bar)
    {
        float step = maxVal / bar.Length;
        for (int i = 0; i < bar.Length; i++)
        {
            float min = i * step;
            float max = (i + 1) * step;
            bar[i].minValue = min;
            bar[i].maxValue = max;
            bar[i].value = Mathf.Min(max, startingVal);
            bar[i].GetComponent<Animator>().SetBool("Full", bar[i].value == bar[i].maxValue);
        }
    }

    public void SetHealth(float newHealth)
    {
        SetBar(oldHealth, newHealth, ref healthbar);
        oldHealth = newHealth;
    }

    public void SetEnergy(float newEnergy)
    {
        SetBar(oldEnergy, newEnergy, ref energybar);
        oldEnergy = newEnergy;
    }

    private void SetBar(float oldVal, float newVal, ref Slider[] bar)
    {
        float step = bar[bar.Length - 1].value / bar.Length;
        int oldI = Mathf.Min(Mathf.FloorToInt(oldVal / step), bar.Length - 1);
        int newI = Mathf.Min(Mathf.FloorToInt(newVal / step), bar.Length - 1);

        bar[newI].value = newVal;
        if (newI < oldI) // decrease across bars
        {
            for (int i = oldI; i > newI; i--)
            {
                bar[i].value = bar[i].minValue;
                bar[i - 1].GetComponent<Animator>().SetBool("Full", false);
            }
        }
        else if (newI > oldI) // increase across bars
        {
            for (int i = oldI; i < newI; i++)
            {
                bar[i].value = bar[i].maxValue;
                bar[i].GetComponent<Animator>().SetBool("Full", true);
            }
        }
        else if (newI == oldI && newVal < oldVal) // decrease within one bar
        {
            bar[newI].GetComponent<Animator>().SetBool("Full", false);
        }
        if (newVal == bar[bar.Length - 1].maxValue) // completely full
        {
            bar[newI].GetComponent<Animator>().SetBool("Full", true);
        }
    }

    public void UpdateAbilityIcons(BiomeType currentBiome)
    {
        switch (currentBiome)
        {
            case BiomeType.FOREST:
            case BiomeType.MEADOW:
            case BiomeType.WATER:
                int cur = (int)currentBiome - 1;
                Debug.Assert(cur >= 0 && cur < icons.Length);

                for (int i = 0; i < icons.Length; i++)
                {
                    if (i == cur) icons[i].GetComponent<Animator>().SetBool("Active", true);
                    else icons[i].GetComponent<Animator>().SetBool("Active", false);
                }
                break;
            default:
                for (int i = 0; i < icons.Length; i++)
                {
                    icons[i].GetComponent<Animator>().SetBool("Active", false);
                }
                break;
        }
    }

    public void CastAbility(MainAbility ability)
    {
        if (ability is ForestAbility)
        {
            StartCoroutine(StartAbilityCoolDown(ability, 0));
        }
        else if (ability is MeadowAbility)
        {
            StartCoroutine(StartAbilityCoolDown(ability, 1));
        }
    }

    private IEnumerator StartAbilityCoolDown(MainAbility ability, int idx)
    {
        while (!ability.IsReady)
        {
            icons[idx].fillAmount = ability.ChargedInPercent();
            yield return new WaitForEndOfFrame();
        }
        icons[idx].GetComponent<Animator>().SetTrigger("Ready");
        yield return null;
    }

    public void AbilityNotReady(MainAbility ability)
    {
        icons[0].GetComponent<Animator>().SetTrigger("NotReady");
    }

}
