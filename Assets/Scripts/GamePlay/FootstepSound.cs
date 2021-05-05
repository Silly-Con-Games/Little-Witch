using UnityEngine;

public class FootstepSound : MonoBehaviour
{
	public float stepLengthSqr;

	private Vector3 lastPos;

	void Start() {
		lastPos = transform.position;
	}

	void FixedUpdate() {
		Vector3 diff = transform.position - lastPos;
		if (diff.sqrMagnitude > stepLengthSqr) {
			lastPos = transform.position;

			FMODUnity.RuntimeManager.PlayOneShot("event:/test/step");
		}
	}
}
