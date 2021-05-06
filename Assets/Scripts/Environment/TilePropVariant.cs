using System.Collections;
using UnityEngine;

public class TilePropVariant : MonoBehaviour {
	public MeshRenderer mesh;
	public BiomeType type;

	public Vector3 origScale;

	public BiomeType GetBiomeType() { 
		return type;
	}

	public void ResetColors() {
		ColorUtils.SetSaturation(mesh, 1f);
	}

	public virtual void Spawn(bool immediate) {
		gameObject.SetActive(true);
		ResetColors();
		if (immediate) {
			mesh.gameObject.transform.localScale = origScale;
			return;
		}

		StartCoroutine(SpawnCoroutine());
	}

	public virtual void Despawn(bool immediate) {
		if (immediate || !gameObject.activeInHierarchy) { 
			mesh.gameObject.transform.localScale = Vector3.zero;
			mesh.gameObject.SetActive(false);
			return;
		}

		StartCoroutine(DespawnCoroutine());
	}

	public virtual void Saturate(bool immediate) {
		if (immediate) {
			ResetColors();
			return;
		}

		StartCoroutine(SaturateCoroutine());
	}

	public virtual void Desaturate(bool immediate) {
		if (immediate) {
			ColorUtils.SetSaturation(mesh, 0f);
			return;
		}

		StartCoroutine(DesaturateCoroutine());
	}

	#region Coroutines

	IEnumerator SpawnCoroutine() {
		mesh.gameObject.transform.localScale = Vector3.zero;

		for (float f = 0f; f < 1f; f += 0.01f) {
			mesh.gameObject.transform.localScale = origScale * f;
			yield return null;
		}
	}

	IEnumerator DespawnCoroutine() {
		Vector3 initSize = mesh.gameObject.transform.localScale;
		for (float f = 1f; f >= 0f; f -= 0.01f) {
			mesh.gameObject.transform.localScale = initSize * f;
			yield return null;
		}
		gameObject.SetActive(false);
	}

	IEnumerator DesaturateCoroutine() {
		for (float progress = 1f; progress >= 0f; progress -= 0.01f) {
			ColorUtils.SetSaturation(mesh, progress);
			yield return null;
		}
	}

	IEnumerator SaturateCoroutine() {
		for (float progress = 0f; progress < 1f; progress += 0.01f) {
			ColorUtils.SetSaturation(mesh, progress);
			yield return null;
		}
	}

	#endregion
}
