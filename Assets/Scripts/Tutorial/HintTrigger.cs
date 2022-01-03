using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    public float hintDelay = 0;
    public EAbilityType hintToDisplay = EAbilityType.None;

    private HintSpawner hintSpawner;

    private void Awake()
    {
        hintSpawner = FindObjectOfType<HintSpawner>();
        if (hintSpawner == null)
            Debug.LogError("HintSpawner not found in the scene");
    }

    private void OnTriggerEnter(Collider other)
    {
        var ot = other.GetComponent<IObjectType>();
        if (ot != null && ot.GetObjectType() == EObjectType.Player)
        {
            StartCoroutine(TryDisplayHintCor());
        }
    }

    IEnumerator TryDisplayHintCor()
    {
        if (hintToDisplay != EAbilityType.None && hintSpawner != null)
        {
            yield return new WaitForSeconds(hintDelay);
            hintSpawner.SpawnHint(hintToDisplay);
        }
    }
}
