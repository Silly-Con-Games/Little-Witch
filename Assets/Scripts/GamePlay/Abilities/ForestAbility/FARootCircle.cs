using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FARootCircle : MonoBehaviour
{
    public Transform animationParent;
    public Animation singleThornPrefab;
    public OnTriggerEnterEvent circleCollider;


    public void Init(float radius, int density, UnityAction<Collider> onHit)
    {
        //animationParent.localScale = animationParent.localScale * radius;
        circleCollider.ontriggerenter.AddListener(onHit);
        circleCollider.GetComponent<SphereCollider>().radius = radius;
        for(int i = 0; i < density; ++i)
        {
            var thorn = Instantiate(singleThornPrefab, animationParent).transform;
            var randPos = Random.insideUnitCircle * radius;
            thorn.localPosition = new Vector3(randPos.x, 0, randPos.y);
            //thorn.localRotation = Random.rotationUniform; 
        }
        StartCoroutine(LifeTimeCourotine());
    }


    IEnumerator LifeTimeCourotine()
    {
        yield return new WaitForSeconds(1);
        circleCollider.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        circleCollider.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
