using UnityEngine;
using FMODUnity;

public class ButtonSound : MonoBehaviour
{
    public void Click() {
		RuntimeManager.PlayOneShot("event:/menu_click");
	}
}
