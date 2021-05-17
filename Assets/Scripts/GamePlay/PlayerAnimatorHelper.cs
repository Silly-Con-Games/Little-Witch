using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorHelper : MonoBehaviour
{
    public void FinishDyingAnim()
    {
        //GetComponentInParent<PlayerController>().enabled = true;
        Destroy(GetComponentInParent<PlayerController>().gameObject);
    }
}
