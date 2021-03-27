using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationControl_Tool : MonoBehaviour
{
    public Animation[] zKey;
    public Animation[] xKey;
    public UnityEvent CEvent;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            playAnimations(zKey);
        if (Input.GetKeyDown(KeyCode.X))
            playAnimations(xKey);
        if (Input.GetKeyDown(KeyCode.C))
            CEvent.Invoke();    
    }

    void playAnimations(Animation[] animations)
    {
        foreach(var an in animations)
            an.Play();
    }
}
