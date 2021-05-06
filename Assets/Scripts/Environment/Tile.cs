using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour {

	public BiomeType wantedType;
	private BiomeType type;

    public MeshRenderer mesh;
	public TileColors colors;
	public float waterDepression;

	public List<TileProp> props;
	public Grass grass;

	public Tile[] neighbours;

	public MapInfo mapInfo { get; set; }

	private float morphSpeed = 2;

	void Awake() {
		Vector3[] directions = new Vector3[6];
		directions[0] = new Vector3(1, 0, 2);
		directions[1] = new Vector3(1, 0, 0);
		directions[2] = new Vector3(1, 0, -2);
		directions[3] = new Vector3(-1, 0, 2);
		directions[4] = new Vector3(-1, 0, 0);
		directions[5] = new Vector3(-1, 0, -2);

		neighbours = new Tile[6];
		RaycastHit hit;
		Vector3 bitLower = new Vector3(0, -0.2f, 0);
		for (int i = 0; i < directions.Length; i++) {
			if (Physics.Raycast(transform.position + bitLower, directions[i], out hit, 2f, LayerMask.GetMask("Tile"))) {
				Tile tile = hit.transform.gameObject.GetComponent<Tile>();
                if (tile) {
				    neighbours[i] = hit.transform.gameObject.GetComponent<Tile>();
				}
			}
		}

		mapInfo = new MapInfo();
	}

    private void Start()
    {
		type = wantedType;
	}

    // to detect changes in edit mode
    [Button("Setup", "Setup", false)] public string input1;
	public void Setup() {
		Morph(wantedType, true);
	}

	public bool WantsToBeSet() {
		return wantedType != type;
	}

	public BiomeType GetBiomeType() {
		return type;
	}

	public void Morph(BiomeType target, bool immediate) {

		if (type == target) {
			Debug.LogWarning("Tile already has this type!", this);
		}

		//grass.Morph(target, immediate);
		foreach (var prop in props) {
			prop.Morph(target, immediate);
		}

		// die
		if (target == BiomeType.DEAD) {
			if (immediate) {
				ColorUtils.SetSaturation(mesh, 0f);
				type = target;
				return;
			}

			StartCoroutine(DieCoroutine());
			return;
		}

		// morph - revive
		if (immediate) {
			if (target == BiomeType.WATER) {
				SetWater(mesh, 1f);
				SetHeight(-waterDepression);
			} else {
				SetWater(mesh, 0f);
				SetHeight(0f);
			}
			ColorUtils.SetSaturation(mesh, 1f);
			ColorUtils.SetColor(mesh, GetColor(target));
			type = target;
			return;
		}

		StartCoroutine(MorphCoroutine(target));
	}

	public Tile[] GetNeighbours() {
		return neighbours;
	}

	public Tile GetNeighbour(int index) {
		return neighbours[index];
	}

	public Color GetColor(BiomeType type) {
		switch (type) {
			case BiomeType.FOREST:
				return colors.forest.Evaluate(Random.value);
			case BiomeType.MEADOW:
				return colors.plain.Evaluate(Random.value);
			case BiomeType.WATER:
				return colors.water;
			default:
				throw new System.Exception("Unknown BiomeType");
		}
	}

	IEnumerator DieCoroutine() {
		for (float progress = 1f; progress >= 0f; progress -= morphSpeed * Time.deltaTime) {
			ColorUtils.SetSaturation(mesh, progress);

			if (progress > 0.5f) {
				type = BiomeType.DEAD;
			}

			yield return null;
		}
	}

	IEnumerator MorphCoroutine(BiomeType target) {

		bool alsoSaturate = type == BiomeType.DEAD;

		bool toWater = target == BiomeType.WATER;
		float initWater = mesh.sharedMaterials[1].GetFloat("_IsWater");
		float initHieght = transform.position.y;

		Color from = mesh.sharedMaterials[0].color;
		Color to = GetColor(target);

		for (float progress = 0f; progress < 1f; progress += morphSpeed * Time.deltaTime ) {
			if (toWater) {
				SetWater(mesh, Mathf.Lerp(initWater, 1f, progress));
				SetHeight(Mathf.Lerp(initHieght, -waterDepression, progress));
			} else {
				SetWater(mesh, Mathf.Lerp(initWater, 0f, progress));
				SetHeight(Mathf.Lerp(initHieght, 0f, progress));
			}
			ColorUtils.SetColor(mesh, Color.Lerp(from, to, progress));

			if (alsoSaturate) {
				ColorUtils.SetSaturation(mesh, 1f - progress);
			}

			if (progress > 0.5f) {
				type = target;
			}

			yield return null;
		}
	}

	private void SetWater(MeshRenderer mesh, float value) {
		Material[] tempMaterials = new Material[mesh.sharedMaterials.Length];
		for (int i = 0; i < tempMaterials.Length; i++) {
			tempMaterials[i] = new Material(mesh.sharedMaterials[i]);
			tempMaterials[i].SetFloat("_IsWater", value);
		}
		mesh.sharedMaterials = tempMaterials;
	}

	private void SetHeight(float height) {
		transform.position = new Vector3(transform.position.x, height, transform.position.z);
	}
}
