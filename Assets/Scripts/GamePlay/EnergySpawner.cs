using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergySpawner : MonoBehaviour
{
    public Energy prefab;
    private Energy spawnedEnergy;

    // Update is called once per frame
    void Update()
    {
        if(spawnedEnergy == null)
        {
            spawnedEnergy = Instantiate(prefab);
            spawnedEnergy.transform.position = transform.position;
        }
    }
}
