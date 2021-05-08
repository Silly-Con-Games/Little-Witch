using System.Collections;
using UnityEngine;
using UnityEngine.VFX;


public class WAWave : MonoBehaviour
{
    public VisualEffect wave;
    public float chargeTime = 0.4f;
    public float speed = 5f;
    public OnTriggerEnterEvent onCollide;
    private Transform transCollider;
    private Collider vfxCollider;
    private bool shouldMove = false;
    private float start;
    // Start is called before the first frame update
    void Start()
    {
        onCollide.ontriggerenter.AddListener(OnHit);
        transCollider = onCollide.transform;
        vfxCollider = onCollide.GetComponent<Collider>();
        wave.SetFloat("ChargeTime", chargeTime);
        wave.SetFloat("Speed", speed);
        wave.SendEvent("ChargeStart");
        StartCoroutine(SendLateEvent());

    }

    private IEnumerator SendLateEvent()
    {
        yield return new WaitForSeconds(chargeTime);
        wave.SendEvent("ChargeEnd");
        shouldMove = true;
        StartCoroutine(MoveCollider());
        yield return new WaitForSeconds(0.5f);

        wave.SendEvent("Stop");
        yield return new WaitForSeconds(0.4f);
        shouldMove = false;
        yield return new WaitForSeconds(1f);

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
        IPushable pushable = collider.gameObject.GetComponent<IPushable>();
        if(pushable != null)
        {
            Vector3 force = (collider.transform.position - transform.position).normalized * speed;
            pushable.ReceivePush(force, 0.9f - (Time.time - start));
        }

        IObjectType objectType = collider.gameObject.GetComponent<IObjectType>();
        if(objectType?.GetObjectType() == EObjectType.Projectile)
        {
            Destroy(collider.gameObject);
        }
    }
}
