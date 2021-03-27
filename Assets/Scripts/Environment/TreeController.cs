using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : TileProp
{
    public MeshRenderer normal;
	public MeshRenderer arid;

	private bool isArid;
	private Color[] originalColors;
	private Vector3 originalScale;

	void Start() {
		originalScale = normal.gameObject.transform.localScale;
	}

	public void Spawn(bool immediate, bool isArid) {
		this.isArid = isArid;

		normal.gameObject.SetActive(!isArid);
		arid.gameObject.SetActive(isArid);
		
		if (!immediate) {
			base.Spawn(isArid ? arid.gameObject : normal.gameObject, originalScale);
		}
	}

	public void Despawn(bool immediate) {
		GameObject tree = isArid ? arid.gameObject : normal.gameObject;
		if (immediate) {
			tree.SetActive(false);
			return;
		}
		base.Despawn(isArid ? arid.gameObject : normal.gameObject, false);
	}

	public void Die(bool immediate) {
		MeshRenderer mesh = isArid ? arid : normal;
		originalColors = new Color[mesh.sharedMaterials.Length];
		for (int i = 0; i < mesh.sharedMaterials.Length; i++) {
			originalColors[i] = mesh.sharedMaterials[i].color;
		}

		if (immediate) {
			Color[] desaturated = new Color[originalColors.Length];
			for (int i = 0; i < desaturated.Length; i++) {
				desaturated[i] = ColorUtils.Desaturate(originalColors[i], 0f);
			}
			ColorUtils.SetColor(mesh, desaturated);
		} else {
			base.Desaturate(mesh);
		}
	}

	public void Revive(bool immediate) {
		MeshRenderer mesh = isArid ? arid : normal;

		if (immediate) {
			ColorUtils.SetColor(mesh, originalColors);
		} else {
			base.Saturate(mesh, originalColors);
		}
	}
}
