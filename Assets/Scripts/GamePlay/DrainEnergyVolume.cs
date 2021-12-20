using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainEnergyVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null)
            player.energy.Set(0);
    }
}
