using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProp : MonoBehaviour
{
	public List<TilePropVariant> variants;

	private TilePropVariant current;

	void Awake() {
		foreach (var variant in variants) { 
			if (variant.gameObject.activeInHierarchy) { 
				current = variant;
			}
		}
	}

	public void Morph(BiomeType type, bool immediate) {
		// die
		if (type == BiomeType.DEAD) {
			if (current != null) {
				current.Desaturate(immediate);
			}
			return;
		}

		// revive
		if (current != null && current.GetBiomeType() == type) {
			current.Saturate(immediate);
			return;
		}

		// morph - despawn
		foreach (var variant in variants) {
			if (variant.gameObject.activeInHierarchy) {
				variant.Despawn(immediate);
			}
		}

		// morph - spawn
		current = null;
		foreach (var variant in variants) {
			if (variant.GetBiomeType() == type) {
				current = variant;
				current.Spawn(immediate);
				break;
			}
		}
	}
}
