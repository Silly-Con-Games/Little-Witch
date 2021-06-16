using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveDefinition 
{
    public List<SwarmDefinition> swarmDefinitions;
    public float preparationTime;

    public void Spawn()
    {
        foreach (var swarm in swarmDefinitions)
            swarm.Spawn();
    }
}
