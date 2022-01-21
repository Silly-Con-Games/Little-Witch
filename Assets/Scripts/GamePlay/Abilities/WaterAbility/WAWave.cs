using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Config;
using System.Collections.Generic;
using Assets.Scripts.GameEvents;

public class WAWave : MonoBehaviour
{
    public VisualEffect wave;
    private float chargeTime = 0.2f;
    private float speed = 10f;
    private float waveDuration = 0.5f;
    public OnTriggerEnterEvent onCollide;
    private Transform transCollider;
    private Collider vfxCollider;
    private bool shouldMove = false;
    private float start;
    HashSet<IPushable> hashSet = new HashSet<IPushable>();

    public void Init(ref WaterAbilityConfig conf)
    {
        hashSet.Clear();
        onCollide.ontriggerenter.AddListener(OnHit);
        transCollider = onCollide.transform;
        vfxCollider = onCollide.GetComponent<Collider>();

        chargeTime = conf.chargeTime;
        speed = conf.waveSpeed;
        waveDuration = conf.waveDuration;

        wave.SetFloat("ChargeTime", chargeTime);
        wave.SetFloat("WaveDuration", waveDuration);
        wave.SetFloat("Speed", speed);
        wave.SendEvent("ChargeStart");
        StartCoroutine(SendLateEvent());
    }

    private IEnumerator SendLateEvent()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/witch/abilities/water_ability", transform.position);
        float halfduration = waveDuration / 2;
        yield return new WaitForSeconds(chargeTime);
        wave.SendEvent("ChargeEnd");
        shouldMove = true;
        StartCoroutine(MoveCollider());
        yield return new WaitForSeconds(halfduration);

        wave.SendEvent("Stop");
        yield return new WaitForSeconds(halfduration);
        shouldMove = false;
        yield return new WaitForSeconds(halfduration);

        Destroy(gameObject);
    }

    private IEnumerator MoveCollider()
    {
        vfxCollider.enabled = true;
        start = Time.time;
        while (shouldMove)
        {            
            transCollider.localPosition += Vector3.forward * Time.deltaTime * speed;
            transCollider.localScale += new Vector3(Time.deltaTime * speed * 0.7f, 0, 0);
            yield return null;
        }
        vfxCollider.enabled = false;
    }

    private void OnHit(Collider collider)
    {
        IPushable pushable = collider.GetComponent<IPushable>();
        if(pushable != null && !hashSet.Contains(pushable))
        {
            hashSet.Add(pushable);
            GameEventQueue.QueueEvent(new WaterAbilityEvent(pushedEnemy: true));
            Vector3 force = (collider.transform.position - transform.position).normalized * speed;
            force.y = 10;
            Debug.Log($"Wave force multiplied by {(1 - (Time.time - start) / (waveDuration*2)) }");
            pushable.ReceivePush(force * (1 - (Time.time - start)/ (waveDuration * 2)), (1 - (Time.time - start)));
        }

        IObjectType objectType = collider.GetComponent<IObjectType>();
        if(objectType?.GetObjectType() == EObjectType.Projectile)
        {
            GameEventQueue.QueueEvent(new WaterAbilityEvent(killedProjectile: true));

            Destroy(collider.gameObject);
        }
    }
}
