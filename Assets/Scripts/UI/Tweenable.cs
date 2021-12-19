using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tweenable : MonoBehaviour
{
    private Image img;

    private float startValue;
    private float targetValue;
    private float targetTime;
    private float timeLeft;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            Color c = img.color;
            c.a = Mathf.Lerp(startValue, targetValue, 1 - timeLeft / targetTime);
            img.color = c;
        }
    }

    public void TweenColorAlpha(float targetValue, float targetTime)
    {
        //Debug.Log("tweening img alpha to " + targetValue);

        startValue = img.color.a;
        this.targetValue = targetValue;
        this.targetTime = targetTime;
        timeLeft = targetTime;
    }
}
