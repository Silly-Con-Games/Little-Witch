using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Hint
{
    public AbilityType abilityType;
    public string hintText;
    public string buttonToPress;
    [HideInInspector] public HintController instance = null;
    [HideInInspector] public bool isActive = false;
}

public class HintSpawner : MonoBehaviour
{
    [SerializeField] private List<Hint> hints; 

    [SerializeField] private HintController hintPrefab;
        
    public void SpawnHint(AbilityType abilityType)
    {
        Debug.Log("spawning hint " + abilityType);

        Hint h = hints.Find(x => x.abilityType == abilityType);
        if (h == null) return;

        h.isActive = true;
        h.instance = Instantiate(hintPrefab, transform);
        h.instance.SetHintText(h.hintText, h.buttonToPress);
    }

    public void HandleKeyPress(AbilityType abilityType)
    {
        Debug.Log("handling key press of ability of type " + abilityType);
        Hint h = hints.Find(x => x.abilityType == abilityType);
        if (h == null || h.instance == null) return;

        h.isActive = false;
        h.instance.HideHint();
        h.instance = null;
    }


    // debug
    public void SpawnTestHint()
    {
        SpawnHint((AbilityType)UnityEngine.Random.Range(0, (int)AbilityType.Count));
        //SpawnHint(AbilityType.Main);
    }

}
