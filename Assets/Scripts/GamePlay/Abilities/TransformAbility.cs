using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TransformAbility
{
	public Transform origin;
	public PlayerController parent;

	public TransformConfig conf;

	private float lastUsedTime = float.NegativeInfinity;

	public void Transform(BiomeType target) {
		lastUsedTime = Time.time;

		if(parent.mapController.Transform(origin.position, target))
			parent.energy.UseEnergy(conf.energyCost);
	}

	public bool IsReady() {
		if (!parent.energy.HasEnough(conf.energyCost))
			return false;
		else
			return Time.time - lastUsedTime > conf.cooldown;
	}
	
}
