using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformEnvironment : MonoBehaviour
{
    public GameObject sphere;

	public TileColors colors;

	private TileType type;
	private bool hasTile;
	private Vector3 originalScale;

	void Start() {
		hasTile = false;
		type = TileType.FOREST;
		originalScale = sphere.transform.localScale;
		sphere.SetActive(false);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			
			RaycastHit hit;
			if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, LayerMask.GetMask("Tile"))) {
				Tile tile = hit.transform.parent.gameObject.GetComponent<Tile>();

				if (hasTile) {
					hasTile = false;
					tile.Morph(false, type);
					foreach (Tile ngb in tile.GetNeighbours()) {
						if (ngb != null) {
							ngb.Morph(false, type);
						}
					}
					
					StartCoroutine(HideCoroutine());
				} else if (tile.tileState != TileState.DEAD) {
					hasTile = true;
					tile.Die(false);
					type = tile.tileType;
					foreach (Tile ngb in tile.GetNeighbours()) {
						if (ngb != null) {
							ngb.Die(false);
						}
					}

					sphere.GetComponent<MeshRenderer>().material.color = tile.GetColor(type);
					StartCoroutine(ShowCoroutine());
				}
			}
		}
	}

	IEnumerator ShowCoroutine() {
		sphere.SetActive(true);
		for (float f = 0f; f < 1f; f += 0.01f) {
			sphere.transform.localScale = originalScale * f;
			yield return null;
		}
	}

	IEnumerator HideCoroutine() {
		for (float f = 1f; f >= 0f; f -= 0.01f) {
			sphere.transform.localScale = originalScale * f;
			yield return null;
		}
		sphere.SetActive(false);
	}
}
