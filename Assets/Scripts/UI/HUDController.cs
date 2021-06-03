using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HUDController : MonoBehaviour
{
    // health and energy
    [SerializeField] private GameObject healthParent;
    [SerializeField] private Slider healthbar;
    private Slider[] healthbars;
    private float oldHealth;
    [SerializeField] private GameObject energyParent;
    [SerializeField] private Slider energybar;
    private Slider[] energybars;
    private float oldEnergy;

    // ability icons
    [SerializeField] private Image[] icons;
    private Image[] iconsInner;
    private Color[] iconColors;

    // wave info
    [SerializeField] private Image waveTimer;
    [SerializeField] private Animator waveDefeated;

    public PlayerController playerController;

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

    #region Health and Energy

    public void SetUpHealth(float startingHealth, float maxHealth, int barCount = 5)
    {
        healthbars = new Slider[barCount];
        healthbars[0] = healthbar;
        for (int i = 1; i < barCount; i++)
        {
            healthbars[i] = Instantiate(healthbar, healthParent.transform);
        }
        SetUpBar(startingHealth, maxHealth, ref healthbars);
        oldHealth = startingHealth;
    }

    public void SetUpEnergy(float startingEnergy, float maxEnergy, int barCount = 3)
    {
        energybars = new Slider[barCount];
        energybars[0] = energybar;
        for (int i = 1; i < barCount; i++)
        {
            energybars[i] = Instantiate(energybar, energyParent.transform);
        }

        SetUpBar(startingEnergy, maxEnergy, ref energybars);
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
        SetBar(oldHealth, newHealth, ref healthbars);
        oldHealth = newHealth;
    }

    public void SetEnergy(float newEnergy)
    {
        SetBar(oldEnergy, newEnergy, ref energybars);
        oldEnergy = newEnergy;
    }

    private void SetBar(float oldVal, float newVal, ref Slider[] bar)
    {
        if (oldVal == newVal) return;

        float step = bar[bar.Length - 1].maxValue / bar.Length;
        int oldI = Mathf.Min(Mathf.FloorToInt(oldVal / step), bar.Length - 1);
        int newI = Mathf.Min(Mathf.FloorToInt(newVal / step), bar.Length - 1);

        // decrease:
        if (oldVal > newVal)
        {
            for (int i = newI; i <= oldI; i++)
            {
                bar[i].value = Mathf.Max(newVal, bar[i].minValue);
                bar[i].GetComponent<Animator>().SetBool("Full", false);
            }
        }
        // increase
        else
        {
            for (int i = oldI; i <= newI; i++)
            {
                bar[i].value = Mathf.Min(newVal, bar[i].maxValue);
                bar[i].GetComponent<Animator>().SetBool("Full", newVal >= bar[i].maxValue);
            }
        }
    }

    public void NotEnoughEnergy()
    {
        for (int i = 0; i < energybars.Length; i++)
        {
            energybars[i].GetComponent<Animator>().SetTrigger("NotEnoughEnergy");
        }
    }

    #endregion

    #region Ability Icons

    public void TransformBiome(string type)
    {
        if (type == "forest")
        {
            playerController.OnTransformForest(null);
        }
        else if (type == "meadow")
        {
            playerController.OnTransformMeadow(null);
        }
        else if (type == "water")
        {
            playerController.OnTransformWater(null);
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
        switch (ability)
        {
            case ForestAbility a:
                StartCoroutine(StartAbilityCoolDown(ability, 0));
                break;
            case MeadowAbility a:
                StartCoroutine(StartAbilityCoolDown(ability, 1));
                break;
            case WaterAbility a:
                StartCoroutine(StartAbilityCoolDown(ability, 2));
                break;
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
        switch (ability)
        {
            case ForestAbility a:
                icons[0].GetComponent<Animator>().SetTrigger("NotReady");
                break;
            case MeadowAbility a:
                icons[1].GetComponent<Animator>().SetTrigger("NotReady");
                break;
            case WaterAbility a:
                icons[2].GetComponent<Animator>().SetTrigger("NotReady");
                break;
        }
    }

    #endregion

    #region Wave Info

    public IEnumerator ShowTimeTillNextWave(float duration)
    {
        waveTimer.gameObject.SetActive(true);
        TextMeshProUGUI waveTimerText = waveTimer.GetComponentInChildren<TextMeshProUGUI>();
        float timeStart = Time.time;
        float time;
        while ((time = duration - (Time.time - timeStart) )> 0)
        {

            waveTimerText.text = $"next wave in {Mathf.Ceil(time).ToString("0")}s";
            yield return null;
        }
        waveTimer.gameObject.SetActive(false); 
    }

    public void ShowWaveDefeated()
    {
        Debug.Log("wave defeted ui");
        waveDefeated.SetTrigger("WaveDefeated");
    }

    #endregion
}
