using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[SelectionBase]
[ExecuteAlways]
public class Tile : MonoBehaviour {

	public TileType tileType;
	public TileState tileState;

    public MapInfo mapInfo { get; set; }

    public MeshRenderer mesh;
	public TileColors colors;

	public TreeController tree;
	public List<GrassController> grasses;
	public GameObject grassPrefab;

	private bool change = false;
	public TileType wantedType;
	public bool isDead = false;
	
	public Tile[] neighbours;

	void Awake() {
		Vector3[] directions = new Vector3[6];
		directions[0] = new Vector3(1, 0, 2);
		directions[1] = new Vector3(1, 0, 0);
		directions[2] = new Vector3(1, 0, -2);
		directions[3] = new Vector3(-1, 0, 2);
		directions[4] = new Vector3(-1, 0, 0);
		directions[5] = new Vector3(-1, 0, -2);

        mapInfo = new MapInfo();
        
		neighbours = new Tile[6];
		RaycastHit hit;
		Vector3 bitLower = new Vector3(0, -0.2f, 0);
		for (int i = 0; i < directions.Length; i++) {
			if (Physics.Raycast(transform.position + bitLower, directions[i], out hit, 2f, 7)) {
				Tile tile = hit.transform.parent.gameObject.GetComponent<Tile>();
                if (tile)
				    neighbours[i] = hit.transform.parent.gameObject.GetComponent<Tile>();
			}
		}
	}
	
	// used only for changes in edit mode
	void Update() {
		if (change) {
			change = false;

			if (isDead && tileState != TileState.DEAD) {
				Die(!EditorApplication.isPlayingOrWillChangePlaymode);
			}
			if ((!isDead && tileState == TileState.DEAD) || wantedType != tileType) {
				isDead = false;
				Morph(!EditorApplication.isPlayingOrWillChangePlaymode, wantedType);
			}
		}
	}

	// to detect changes in edit mode
	void OnValidate() {
		change = true;
	}

	public Tile[] GetNeighbours() {
		return neighbours;
	}

	public void Die(bool immediate) {
		if (immediate) {
			ColorUtils.SetColor(mesh, ColorUtils.Desaturate(mesh.sharedMaterial.color, 0f));
			tileState = TileState.DEAD;
		} else {
			StartCoroutine(DieCoroutine());
		}

		if (tree != null) {
			tree.Die(immediate);
		}
		foreach (GrassController grass in grasses) {
			grass.Die(immediate);
		}
	}

	public void Morph(bool immediate, TileType target) {
		if (tileType == target) {
			if (tree != null) {
				tree.Revive(immediate);
			}
			foreach (GrassController grass in grasses) {
				grass.Revive(immediate);
			}
		} else {
			ProcessTrees(immediate, target);
			ProcessGrass(immediate, target);
		}

		if (immediate) {
			ColorUtils.SetColor(mesh, GetColor(target));
			tileType = target;
			tileState = TileState.ALIVE;
		} else {
			StartCoroutine(MorphCoroutine(target));
		}
	}

	private void ProcessTrees(bool immediate, TileType target) {
		if (tree != null) {
			tree.Despawn(immediate);

			switch (target) {
				case TileType.FOREST:
					tree.Spawn(immediate, false);
					break;
				case TileType.SAND:
					tree.Spawn(immediate, true);
					break;
			}
		}
	}

	private void ProcessGrass(bool immediate, TileType target) {
		foreach (GrassController grass in grasses) {
			grass.Despawn(immediate);
		}
		grasses.Clear();

		int grassCount = 0;
		switch (target) {
			case TileType.FOREST:
				grassCount = 2;
				break;
			case TileType.PLAIN:
				grassCount = 4;
				break;
		}
		
		for (int i = 0; i < grassCount; i++) {
			GameObject grassObject = Instantiate(grassPrefab);
			grassObject.transform.position = transform.position + (Random.Range(0.3f, 0.7f) * RandomUtils.GetRandomDirection());
			grassObject.transform.rotation = Quaternion.Euler(0f, Random.value * 360f, 0f);
			GrassController grass = grassObject.GetComponent<GrassController>();
			grass.Spawn(immediate, GetColor(target));
			grasses.Add(grass);
		}
	}

	public Color GetColor(TileType type) {
		switch (type) {
			case TileType.FOREST:
				return colors.forest.Evaluate(Random.value);
			case TileType.PLAIN:
				return colors.plain.Evaluate(Random.value);
			case TileType.SAND:
				return colors.sand.Evaluate(Random.value);
			default:
				throw new System.Exception("Unknown TileType");
		}
	}

	IEnumerator DieCoroutine() {
		tileState = TileState.DYING;
		Color originalColor = mesh.sharedMaterial.color;
		for (float progress = 1f; progress >= 0f; progress -= 0.01f) {
			ColorUtils.SetColor(mesh, ColorUtils.Desaturate(mesh.sharedMaterial.color, progress));
			yield return null;
		}

		tileState = TileState.DEAD;
	}

	IEnumerator MorphCoroutine(TileType target) {
		tileState = TileState.CHANGING;
		
		Color from = mesh.sharedMaterial.color;
		Color to = GetColor(target);

		for (float progress = 0f; progress < 1f; progress += 0.01f) {
			ColorUtils.SetColor(mesh, Color.Lerp(from, to, progress));
			yield return null;
		}

		tileType = target;
		tileState = TileState.ALIVE;
	}
}

public enum TileType {
	FOREST,
	PLAIN,
	SAND,
	WATER
}

public enum TileState {
	ALIVE,
	CHANGING,
	DYING,
	DEAD
}
