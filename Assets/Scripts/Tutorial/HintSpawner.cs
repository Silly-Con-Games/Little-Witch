using Assets.Scripts.GameEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Hint
{
    public EAbilityType abilityType;
    public string hintText;
    public string buttonToPress;
    [HideInInspector] public HintController instance = null;
    [HideInInspector] public bool isActive = false;
}

public class HintSpawner : MonoBehaviour
{
    [SerializeField] private List<Hint> hints; 

    [SerializeField] private HintController hintPrefab;

    private Hint activeHint = null;
    private static HintSpawner inst;

    private void Awake()
    {
        inst = this;
        GameEventQueue.AddListener(typeof(BiomeTransformedEvent), OnAbilityCast);
        GameEventQueue.AddListener(typeof(ChargeAbilityEvent), OnAbilityCast);
        GameEventQueue.AddListener(typeof(DashAbilityEvent), OnAbilityCast);
        GameEventQueue.AddListener(typeof(MeleeAbilityEvent), OnAbilityCast);
        GameEventQueue.AddListener(typeof(WaterAbilityEvent), OnAbilityCast);
        GameEventQueue.AddListener(typeof(ForestAbilityEvent), OnAbilityCast);
        GameEventQueue.AddListener(typeof(MeadowAbilityEvent), OnAbilityCast);
    }

    private void OnDestroy()
    {
        inst = null;
        GameEventQueue.RemoveListener(typeof(BiomeTransformedEvent), OnAbilityCast);
        GameEventQueue.RemoveListener(typeof(ChargeAbilityEvent), OnAbilityCast);
        GameEventQueue.RemoveListener(typeof(DashAbilityEvent), OnAbilityCast);
        GameEventQueue.RemoveListener(typeof(MeleeAbilityEvent), OnAbilityCast);
        GameEventQueue.RemoveListener(typeof(WaterAbilityEvent), OnAbilityCast);
        GameEventQueue.RemoveListener(typeof(ForestAbilityEvent), OnAbilityCast);
        GameEventQueue.RemoveListener(typeof(MeadowAbilityEvent), OnAbilityCast);
    }

    void OnAbilityCast(IGameEvent ev)
    {
        switch (ev)
        {
            case BiomeTransformedEvent e:
                if (!e.enemyOrigin)
                    if (e.revive)
                        HandleKeyPress(EAbilityType.Revive);
                    else
                        HandleKeyPress(EAbilityType.Transform);
                break;
            case DashAbilityEvent _:
                HandleKeyPress(EAbilityType.Dash);
                break;
            case ChargeAbilityEvent e:
                if (e.cast)
                {
                    HandleKeyPress(EAbilityType.Charge);
                }
                break;
            case MeleeAbilityEvent e:
                if (e.cast)
                {
                    HandleKeyPress(EAbilityType.Melee);
                }
                break;
            case WaterAbilityEvent e:
                if (e.cast)
                {
                    HandleKeyPress(EAbilityType.Main);
                }
                break;
            case ForestAbilityEvent e:
                if (e.cast)
                {
                    HandleKeyPress(EAbilityType.Main);
                }
                break;
            case MeadowAbilityEvent e:
                if (e.cast)
                {
                    HandleKeyPress(EAbilityType.Main);
                }
                break;
        }
    }

    public static void SpawnHint(EAbilityType abilityType)
    {
        if (inst != null)
            inst.SpawnHintInternal(abilityType);
        else
            Debug.LogError("No Hinst spawner in the scene");
    }

    void SpawnHintInternal(EAbilityType abilityType)
    {
        if (abilityType == EAbilityType.None)
            return;

        Debug.Log("spawning hint " + abilityType);

        Hint h = hints.Find(x => x.abilityType == abilityType);
        if (h == null) return;

        if (activeHint != null)
        {
            activeHint.isActive = false;
            activeHint.instance.HideHint();
            activeHint.instance = null;
        }

        h.isActive = true;
        h.instance = Instantiate(hintPrefab, transform);
        h.instance.SetHintText(h.hintText, h.buttonToPress);
        activeHint = h;
    }

    public void HandleKeyPress(EAbilityType abilityType)
    {
        Debug.Log("handling key press of ability of type " + abilityType);
        Hint h = hints.Find(x => x.abilityType == abilityType);
        if (h == null || h.instance == null) return;

        h.isActive = false;
        h.instance.HideHint();
        h.instance = null;
        activeHint = null;
    }


    // debug
    public void SpawnTestHint()
    {
        SpawnHint((EAbilityType)UnityEngine.Random.Range(0, (int)EAbilityType.Count));
        //SpawnHint(AbilityType.Revive);
    }

}
