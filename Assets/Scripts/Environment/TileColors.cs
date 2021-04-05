using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TileColors")]
public class TileColors : ScriptableObject {
	public Gradient forest;
	public Gradient plain;
	public Gradient sand;
}
