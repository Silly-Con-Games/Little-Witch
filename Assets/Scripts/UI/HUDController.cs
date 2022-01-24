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
    [SerializeField] private Slider energyActual;
    [SerializeField] private Slider energyToBe;
    private float oldEnergy;
    [SerializeField] private Tweenable lowOnHealthScreen;
    private float maxHealth;

    // ability icons
    [SerializeField] private Image[] icons;
    private TextMeshProUGUI[] iconsText;
    private Image[] iconsInner;
    private Color[] iconColors;

    // wave info
    [SerializeField] private Image waveTimer;
    [SerializeField] private Animator waveInfo;
    [SerializeField] private TextMeshProUGUI waveInfoText;

    // game goal
    [SerializeField] private string gameGoalText = "Defeat waves of enemies and protect your home!";

    public PlayerController playerController;

    private void Start()
    {
        // ability icons
        iconsText = new TextMeshProUGUI[icons.Length];
        iconsInner = new Image[icons.Length];
        iconColors = new Color[icons.Length];
        for (int i = 0; i < icons.Length; i++)
        {
            iconsText[i] = icons[i].GetComponentsInChildren<TextMeshProUGUI>()[0];
            iconsInner[i] = icons[i].GetComponentsInChildren<Image>()[1];
            iconColors[i] = iconsInner[i].color;
        }
    }

    public void Init(PlayerController playerController)
    {
        this.playerController = playerController;
        playerController.controlSchemeChanged.AddListener(SwitchText);
    }

    #region Health and Energy

    public void SetUpHealth(float startingHealth, float maxHealth, int barCount = 5)
    {
        if (healthbars == null)
        {
            healthbars = new Slider[barCount];
            healthbars[0] = healthbar;
            for (int i = 1; i < barCount; i++)
            {
                healthbars[i] = Instantiate(healthbar, healthParent.transform);
            }
        }
        SetUpBar(startingHealth, maxHealth, ref healthbars);
        this.maxHealth = maxHealth;
        oldHealth = startingHealth;
    }

    public void SetUpEnergy(float startingEnergy, float maxEnergy)
    {
        energyActual.minValue = 0;
        energyActual.maxValue = maxEnergy;
        energyActual.value = startingEnergy;
        energyActual.GetComponent<Animator>().SetBool("Full", startingEnergy == maxEnergy);

        energyToBe.minValue = 0;
        energyToBe.maxValue = maxEnergy;
        energyToBe.value = startingEnergy;

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

        lowOnHealthScreen.TweenColorAlpha(.5f - (newHealth / maxHealth), .3f); // max aplha is .5, shows up at half health
    }

    public void SetEnergy(float newEnergy)
    {
        energyActual.value = newEnergy;
        energyActual.GetComponent<Animator>().SetBool("Full", newEnergy == energyActual.maxValue);

        energyToBe.value = newEnergy;

        oldEnergy = newEnergy;
    }

    public void ShowEnergyCost(float cost)
    {
        energyToBe.value = energyActual.value - cost;
    }

    public void StopShowEnergyCost()
    {
        energyToBe.value = energyActual.value;
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
        energyActual.GetComponent<Animator>().SetTrigger("NotEnoughEnergy");
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

    // change this to read from input actions
    public void SwitchText(bool gamepad)
    {
        iconsText[0].text = gamepad ? "X" : "1";
        iconsText[1].text = gamepad ? "A" : "2";
        iconsText[2].text = gamepad ? "B" : "3";
    }

    #endregion

    #region Wave Info

    public IEnumerator ShowTimeTillNextWave(float duration, int waveNumber)
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
        ShowWaveStart(waveNumber);
    }

    public void ShowWaveDefeated()
    {
        waveInfoText.text = "Wave Defeated";
        waveInfo.SetTrigger("WaveDefeated");
    }

    private void ShowWaveStart(int waveNumber)
    {
        waveInfoText.text = "Wave " + (waveNumber+1);
        waveInfo.SetTrigger("WaveDefeated");
    }

    public void ShowHintText(string str)
    {
        waveInfoText.text = str;
        waveInfo.SetTrigger("WaveDefeated");
    }

    #endregion

    #region Game goal

    public void ShowGameGoal()
    {
        waveInfoText.text = gameGoalText;
        waveInfo.SetTrigger("WaveDefeated");
    }

    #endregion Game goal
}
