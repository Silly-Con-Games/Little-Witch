using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour {

	public BiomeType wantedType;
	[SerializeField]
	private BiomeType type;
	[SerializeField]
	private BiomeType typeBeforeDeath;

    public MeshRenderer mesh;
	public TileColors colors;
	public float waterDepression;

	[Tooltip("Add in scene map controller")]
	public MapController mapController;

	private IProp prop = null;

	public Tile[] neighbours;

	public MapInfo mapInfo { get; set; }

	private float morphSpeed = 2;

    public bool chosen { get; set; }

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
		for (int i = 0; i < directions.Length; i++) {
			if (Physics.Raycast(transform.position + Vector3.down, directions[i], out hit, 2f, LayerMask.GetMask("Tile"))) {
				Tile tile = hit.transform.gameObject.GetComponent<Tile>();
                if (tile) {
				    neighbours[i] = hit.transform.gameObject.GetComponent<Tile>();
				}
			}
		}

		mapInfo = new MapInfo();

		if(mapController == null)
			mapController = FindObjectOfType<MapController>();
        chosen = false;
    }

	#region editor
	#if UNITY_EDITOR

	// to detect changes in edit mode
	[Button("Setup", "Setup", false)] public string input1;
	public void Setup() {
		if (prop == null)
		{
			prop = GetComponentInChildren<IProp>();
		}
		UnityEditor.EditorApplication.delayCall += () => Morph(wantedType, true);
	}

	// to detect changes in edit mode
	[Button("ReviveInEditor", "ReviveInEditor", false)] public string input2;
	public void ReviveInEditor()
	{
		if (prop == null)
		{
			prop = GetComponentInChildren<IProp>();
		}
		UnityEditor.EditorApplication.delayCall += () => Morph(typeBeforeDeath, true);
	}

	// to detect changes in edit mode
	[Button("Kill", "Kill", false)] public string input3;
	public void Kill()
	{
		if (prop == null)
		{
			prop = GetComponentInChildren<IProp>();
		}
		UnityEditor.EditorApplication.delayCall += () => Morph(BiomeType.DEAD, true);
	}
	#endif
	#endregion

	public void Revive()
    {
		Morph(typeBeforeDeath, false);
	}


	public bool WantsToBeSet() {
		return wantedType != type;
	}

	public BiomeType GetBiomeType() {
		return type;
	}

	public bool HasProp() {
		return prop != null;
	}

	public void Morph(BiomeType target, bool immediate) {

		if (type == target) {
			Debug.LogWarning("Tile already has this type!", this);
			return;
		}
		prop = GetComponentInChildren<IProp>();
		wantedType = target;
		// die
		if (target == BiomeType.DEAD) {
			prop?.Die(immediate);
			typeBeforeDeath = type;
			if (immediate) {
				ColorUtils.SetSaturation(mesh, 0f);
				type = target;
				return;
			}
			StartCoroutine(DieCoroutine());
			return;
		}
        else
        {
			mapController.ReviveTile();
        }

		bool propRevived = false;
		if (prop != null)
        {
			if (type == BiomeType.DEAD && typeBeforeDeath == target)
			{
				prop.Revive(immediate);
				propRevived = true;
			}
            else
            {
				prop.Despawn(immediate);
				prop = null;
			}
		}
        else if(typeBeforeDeath == target)
        {
			propRevived = true;
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
			if(!propRevived)
				TrySpawnProp(true, target);
			type = target;
			return;
		}

		StartCoroutine(MorphCoroutine(target, propRevived));
	}

	public Tile[] GetNeighbours() {
		return neighbours;
	}

	public Tile GetNeighbour(int index) {
		return neighbours[index];
	}

	private void TrySpawnProp(bool immediate, BiomeType biomeType)
    {
		if (prop == null)
        {
			PropAndProbability propPref = mapController.GetProp(biomeType);
			if (propPref != null && propPref.chance >= Random.Range(0.0f, 1.0f))
            {
				prop = Instantiate(propPref.prop, transform).GetComponent<IProp>();
				prop.Spawn(immediate);
            }
		}
    }

	public void SetupPropOnLoad(bool shouldSpawn) {
		if (shouldSpawn) {
			PropAndProbability propPref = mapController.GetProp(type);
			if (propPref != null) {
				prop = Instantiate(propPref.prop, transform).GetComponent<IProp>();
				prop.Spawn(true);
			}
		} else {
			if (prop != null) {
				prop.Despawn(true);
				prop = null;
			}
		}
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
		type = BiomeType.DEAD;
		for (float progress = 1f; progress >= 0f; progress -= morphSpeed * Time.deltaTime) {
			ColorUtils.SetSaturation(mesh, progress);
			yield return null;

		}
		ColorUtils.SetSaturation(mesh, 0);
	}

	IEnumerator MorphCoroutine(BiomeType target, bool propRevived) {

		bool alsoSaturate = type == BiomeType.DEAD;

		bool toWater = target == BiomeType.WATER;
		float initWater = mesh.sharedMaterials[1].GetFloat("_IsWater");
		float initHieght = transform.position.y;

		Color from = mesh.sharedMaterials[0].color;
		Color to = GetColor(target);
		bool isNotSet = true;
		type = target;
		for (float progress = 0f; progress <= 1f; progress += morphSpeed * Time.deltaTime) {
			if (toWater) {
				SetWater(mesh, Mathf.Lerp(initWater, 1f, progress));
				SetHeight(Mathf.Lerp(initHieght, -waterDepression, progress));
			} else {
				SetWater(mesh, Mathf.Lerp(initWater, 0f, progress));
				SetHeight(Mathf.Lerp(initHieght, 0f, progress));
			}
			ColorUtils.SetColor(mesh, Color.Lerp(from, to, progress));

			if (alsoSaturate) {
				ColorUtils.SetSaturation(mesh, progress);
			}

			if (!propRevived && isNotSet && progress > 0.5f) {
				TrySpawnProp(false, target);
				isNotSet = false;
			}

			yield return null;
		}

		if(!propRevived && isNotSet)
			TrySpawnProp(false, target);

		float MaxProgress = 1.0f;
		if (toWater)
		{
			SetWater(mesh, Mathf.Lerp(initWater, 1f, MaxProgress));
			SetHeight(Mathf.Lerp(initHieght, -waterDepression, MaxProgress));
		}
		else
		{
			SetWater(mesh, Mathf.Lerp(initWater, 0f, MaxProgress));
			SetHeight(Mathf.Lerp(initHieght, 0f, MaxProgress));
		}
		ColorUtils.SetColor(mesh, Color.Lerp(from, to, MaxProgress));

		if (alsoSaturate)
		{
			ColorUtils.SetSaturation(mesh, MaxProgress);
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
