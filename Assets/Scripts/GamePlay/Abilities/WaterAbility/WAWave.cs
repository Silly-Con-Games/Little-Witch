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
    // Start is called before the first frame update
    void Start()
    {
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
        yield return new WaitForSeconds(1);

        shouldMove = false;
        wave.SendEvent("Stop");
    }

    private IEnumerator MoveCollider()
    {
        vfxCollider.enabled = true;
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

    }
}
