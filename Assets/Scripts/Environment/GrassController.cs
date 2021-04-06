using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassController : TileProp
{
	public MeshRenderer mesh;

	private Color originalColor;

	public void Spawn(bool immediate, Color color) {
		ColorUtils.SetColor(mesh, color);
		if (!immediate) {
			base.Spawn(gameObject, transform.localScale);
		}
	}

	public void Despawn(bool immediate) {
		if (immediate) {
			DestroyImmediate(gameObject);
			return;
		}
		base.Despawn(gameObject, true);
	}

	public void Die(bool immediate) {
		originalColor = new Color(mesh.sharedMaterial.color.r, mesh.sharedMaterial.color.g, mesh.sharedMaterial.color.b);
		if (immediate) {
			ColorUtils.SetColor(mesh, ColorUtils.Desaturate(originalColor, 0f));
			return;
		}
		base.Desaturate(mesh);
	}

	public void Revive(bool immediate) {
		if (immediate) {
			ColorUtils.SetColor(mesh, originalColor);
			return;
		}
		base.Saturate(mesh, originalColor);
	}
}
