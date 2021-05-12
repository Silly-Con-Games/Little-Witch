using System.Collections;
using UnityEngine;

public class TilePropVariant : MonoBehaviour {
	public MeshRenderer mesh;
	public BiomeType type;

	public Vector3 origScale;

	private float morphSpeed = 2;

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

		for (float progress = 0f; progress <= 1f; progress += morphSpeed * Time.deltaTime) {
			mesh.gameObject.transform.localScale = origScale * progress;

			yield return null;

			if (progress + morphSpeed * Time.deltaTime > 1f) {
				progress = 1f - morphSpeed * Time.deltaTime * 1.1f;
			}
		}
	}

	IEnumerator DespawnCoroutine() {
		Vector3 initSize = mesh.gameObject.transform.localScale;

		for (float progress = 1f; progress >= 0f; progress -= morphSpeed * Time.deltaTime) {
			mesh.gameObject.transform.localScale = initSize * progress;

			yield return null;

			if (progress - morphSpeed * Time.deltaTime < 0f) {
				progress = morphSpeed * Time.deltaTime * 1.1f;
			}
		}
		gameObject.SetActive(false);
	}

	IEnumerator DesaturateCoroutine() {
		for (float progress = 1f; progress >= 0f; progress -= morphSpeed * Time.deltaTime) {
			ColorUtils.SetSaturation(mesh, progress);

			yield return null;

			if (progress - morphSpeed * Time.deltaTime < 0f) {
				progress = morphSpeed * Time.deltaTime * 1.1f;
			}
		}
	}

	IEnumerator SaturateCoroutine() {
		for (float progress = 0f; progress <= 1f; progress += morphSpeed * Time.deltaTime) {
			ColorUtils.SetSaturation(mesh, progress);

			yield return null;

			if (progress + morphSpeed * Time.deltaTime > 1f) {
				progress = 1f - morphSpeed * Time.deltaTime * 1.1f;
			}
		}
	}

	#endregion
}
