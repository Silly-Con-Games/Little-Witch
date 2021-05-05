using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformEnvironment : MonoBehaviour
{
	public float cooldown;

	private float lastUsedTime = float.NegativeInfinity;

	public void Transform(BiomeType target) {
		lastUsedTime = Time.time;

		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, LayerMask.GetMask("Tile"))) {
			Tile tile = hit.transform.gameObject.GetComponent<Tile>();
			tile.Morph(target, false);
			foreach (Tile ngb in tile.GetNeighbours()) {
				if (ngb != null) {
					ngb.Morph(target, false);
				}
			}
		}
	}

	public bool IsReady() {
		return Time.time - lastUsedTime > cooldown;
	}
	
}
