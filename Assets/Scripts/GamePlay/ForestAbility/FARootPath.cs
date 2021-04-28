using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FARootPath : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform animationTransform;
    public Animation singleThornPrefab;
    public OnTriggerEnterEvent movingCollider;

    private int stepsCount;
    private int currentStep = 0;
    private float stepDuration;
    private float stepDistance;
    private float distance;
    private float duration;
    private float stepSize = 0.3f;
    private float timeStart;

    private UnityAction onEnd;
    private bool started = false;
    public void Init(float distance, float duration, UnityAction<Collider> onHit, UnityAction onEnd)
    {
        this.distance = distance;
        this.duration = duration;
        this.onEnd = onEnd;
        stepsCount = (int)(distance / stepSize);
        timeStart = Time.time;
        stepDuration = duration / stepsCount;
        stepDistance = distance / stepsCount;
        started = true;
        movingCollider.gameObject.SetActive(true);
        movingCollider.ontriggerenter.AddListener(onHit);
        StartCoroutine(SpawnPathCourotine());
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            float delta = Mathf.Max((Time.time - (timeStart)) / duration * distance - 1.0f, 0);
            if (delta < distance)
                movingCollider.transform.localPosition = new Vector3(0, 0, delta);
            else
            {
                started = false;
                movingCollider.gameObject.SetActive(false);
                onEnd();
            }
        }
    }

    IEnumerator SpawnPathCourotine()
    {
        float actualDistance = 0;
        float right = 0.35f;
        while (currentStep < stepsCount && actualDistance <= distance)
        {
            actualDistance = currentStep * stepDistance;
            SpawnThorn(actualDistance, -0.85f, right);
            right *= -1;
            yield return new WaitForSeconds(stepDuration);

            currentStep = (int)((Time.time - timeStart) / stepDuration);
        }
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    private void SpawnThorn(float distance, float down, float right)
    {
        var thornTrans = Instantiate(singleThornPrefab, animationTransform).transform;
        thornTrans.localPosition = new Vector3(right, down, distance);
        thornTrans.localRotation = Quaternion.Euler(-90 + Random.Range(-10, 10), Random.Range(0, 180), Random.Range(-10, 10));
    }
}
