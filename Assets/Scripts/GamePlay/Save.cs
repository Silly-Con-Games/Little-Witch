using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Save
{
	public int wave;

	public float energy;
	public float health;
	public SerializableVector witchPosition;

	public List<TileSaveInfo> tiles;
	public List<SerializableVector> energySpheres;
}

[System.Serializable]
public struct TileSaveInfo
{
	public BiomeType type;
	public bool hasProp;
}

[System.Serializable]
public class SerializableVector
{
	public float x, y, z;

	public SerializableVector(Vector3 input) {
		x = input.x;
		y = input.y;
		z = input.z;
	}

	public Vector3 Get() {
		return new Vector3(x, y, z);
	}
}
