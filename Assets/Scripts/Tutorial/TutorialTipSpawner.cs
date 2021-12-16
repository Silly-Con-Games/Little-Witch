using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTipSpawner : MonoBehaviour
{
    [SerializeField] private Dictionary<AbilityType, string> tips;  // ability - tip

    [SerializeField] private TutorialTipController tipPrefab;

    
    public void SpawnTip(AbilityType abilityType)
    {
        if (tips.ContainsKey(abilityType))
        {
            TutorialTipController tipInstance = Instantiate(tipPrefab);
            tipInstance.SetTipText(tips[abilityType]);
        }
        else
        {
            Debug.LogError("Ability tip not found");
        }
    }

}
