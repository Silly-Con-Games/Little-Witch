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

		RaycastHit hit;
		if (Physics.Raycast(origin.position, Vector3.down, out hit, 2f, LayerMask.GetMask("Tile"))) {
			Tile tile = hit.transform.gameObject.GetComponent<Tile>();
			tile.Morph(target, false);
			foreach (Tile ngb in tile.GetNeighbours()) {
				if (ngb != null) {
					ngb.Morph(target, false);
				}
			}

			parent.energy.UseEnergy(conf.energyCost);
		}
	}

	public bool IsReady() {
		if (!parent.energy.HasEnough(conf.energyCost))
			return false;
		else
			return Time.time - lastUsedTime > conf.cooldown;
	}
	
}
