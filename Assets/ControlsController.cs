using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsController : MonoBehaviour
{
    public void Close() {
		gameObject.SetActive(false);
	}
}
