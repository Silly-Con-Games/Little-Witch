using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornSpell : MonoBehaviour
{
    public GameObject thornSequence;
    public GameObject thornFinal;
    public float sequenceStep = 0.3f;

    float currentTimeToNextStep = 0;

    Animation[] sequenceAnims;
    Animation[] final;

    int currentAnim = 0;
    bool isPlaying = false;

    void Start()
    {
        sequenceAnims = thornSequence.GetComponentsInChildren<Animation>();
        final = thornFinal.GetComponentsInChildren<Animation>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            currentTimeToNextStep += Time.deltaTime;
            if(currentTimeToNextStep >= sequenceStep)
            {
                if (currentAnim >= sequenceAnims.Length)
                {
                    foreach (var anim in final)
                        anim.Play();
                    isPlaying = false;
                    return;
                }
                sequenceAnims[currentAnim++].Play();
                currentTimeToNextStep = 0;
            }
        }
    }

    public void Play()
    {
        currentAnim = 0;
        isPlaying = true;
        currentTimeToNextStep = sequenceStep;
    }

    public void Stop()
    {
        isPlaying = false;
    }
}
