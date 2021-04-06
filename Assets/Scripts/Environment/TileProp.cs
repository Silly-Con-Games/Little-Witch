using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileProp : MonoBehaviour
{
	public virtual void Spawn(GameObject go, Vector3 scale) {
		StartCoroutine(SpawnCoroutine(go, scale));
	}

	public virtual void Despawn(GameObject go,  bool destroy) {
		StartCoroutine(DespawnCoroutine(go, destroy));
	}

	public virtual void Desaturate(MeshRenderer mesh) {
		StartCoroutine(DesaturateCoroutine(mesh));
	}

	public virtual void Saturate(MeshRenderer mesh, Color targetColor) {
		StartCoroutine(SaturateCoroutine(mesh, targetColor));
	}

	public virtual void Saturate(MeshRenderer mesh, Color[] targetColors) {
		StartCoroutine(SaturateCoroutine(mesh, targetColors));
	}

	IEnumerator SpawnCoroutine(GameObject go, Vector3 scale) {
		go.transform.localScale = Vector3.zero;

		for (float f = 0f; f < 1f; f += 0.01f) {
			go.transform.localScale = scale * f;
			yield return null;
		}
	}

	IEnumerator DespawnCoroutine(GameObject go, bool destroy) {
		Vector3 initSize = go.transform.localScale;
		for (float f = 1f; f >= 0f; f -= 0.01f) {
			go.transform.localScale = initSize * f;
			yield return null;
		}

		if (destroy) {
			Destroy(go);
		} else {
			go.SetActive(true);
		}
	}

	IEnumerator DesaturateCoroutine(MeshRenderer mesh) {
		Color[] originalColors = new Color[mesh.sharedMaterials.Length];
		for (int i = 0; i < mesh.sharedMaterials.Length; i++) {
			originalColors[i] = mesh.sharedMaterials[i].color;
		}

		Color[] desaturated = new Color[originalColors.Length];
		for (float progress = 1f; progress >= 0f; progress -= 0.01f) {
			for (int i = 0; i < desaturated.Length; i++) {
				desaturated[i] = ColorUtils.Desaturate(originalColors[i], progress);
			}
			ColorUtils.SetColor(mesh, desaturated);
			yield return null;
		}
	}

	IEnumerator SaturateCoroutine(MeshRenderer mesh, Color targetColor) {
		for (float progress = 0f; progress < 1f; progress += 0.01f) {
			ColorUtils.SetColor(mesh, ColorUtils.Desaturate(targetColor, progress));
			yield return null;
		}
	}

	IEnumerator SaturateCoroutine(MeshRenderer mesh, Color[] targetColors) {
		Color[] desaturated = new Color[targetColors.Length];
		for (float progress = 0f; progress < 1f; progress += 0.01f) {
			for (int i = 0; i < desaturated.Length; i++) {
				desaturated[i] = ColorUtils.Desaturate(targetColors[i], progress);
			}
			ColorUtils.SetColor(mesh, desaturated);
			yield return null;
		}
	}
}
