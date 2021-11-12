using UnityEngine;

[ExecuteInEditMode]
public class SnapToHexgrid : MonoBehaviour {

	static readonly float radius = 1;

	static readonly float v = radius * 3f;
	static readonly float h = radius * Mathf.Sqrt(3f);
	static readonly Vector3 shift = new Vector3(h / 2, 0, v / 2);

	void Update()
	{
		#if UNITY_EDITOR
		if (transform.hasChanged) {
			Vector3 newPos1 = Round(transform.position);
			float error1 = (newPos1 - transform.position).sqrMagnitude;

			Vector3 newPos2 = Round(transform.position + shift) - shift;
			float error2 = (newPos2 - transform.position).sqrMagnitude;

			transform.position = error1 < error2 ? newPos1 : newPos2;
		}
		#endif
	}

	private Vector3 Round(Vector3 pos) {
		return new Vector3(Round(pos.x, h), pos.y, Round(pos.z, v));
	}

	private float Round(float x, float step) {
		float rest = x % step;
		if (Mathf.Abs(rest) >= step / 2f) {
			return x - rest + (x > 0 ? step : -step);
		} else {
			return x - rest;
		}
	}
}
