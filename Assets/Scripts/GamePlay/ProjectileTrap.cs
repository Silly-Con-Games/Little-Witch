using Assets.Scripts.GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTrap : MonoBehaviour, IDamagable
{
    public Bullet bulletPrefab;
    public OnTriggerEnterEvent enterEvent;

    [Tooltip("if lower then 0 it will shoot infinitely")]
    public int numberOfShots = 2;
    public float intervalBetweenShots = 2;

    private List<GameObject> bullets = new List<GameObject>();
    Coroutine activeCor;
    private void Start()
    {
        enterEvent.ontriggerenter.AddListener(TrapActivated);
        GameEventQueue.AddListener(typeof(PlayerRespawnedEvent), ResetTrap);
    }

    private void OnDestroy()
    {
        GameEventQueue.RemoveListener(typeof(PlayerRespawnedEvent), ResetTrap);
        if (activeCor != null)
        {
            StopCoroutine(activeCor);
            activeCor = null;
        }
    }

    void TrapActivated(Collider col)
    {
        var p = col.GetComponent<IObjectType>();
        if(p != null && p.GetObjectType() == EObjectType.Player)
        {
            enterEvent.gameObject.SetActive(false);
            activeCor = StartCoroutine(StartShootingCor());
        }
    }

    IEnumerator StartShootingCor()
    {
        if(numberOfShots < 0)
            while (true)
            {
                bullets.Add(Instantiate(bulletPrefab, transform.position, transform.rotation).gameObject);
                yield return new WaitForSeconds(intervalBetweenShots);
            }
        else
            for(int i = 0; i < numberOfShots; i++)
            {
                bullets.Add(Instantiate(bulletPrefab, transform.position, transform.rotation).gameObject);
                yield return new WaitForSeconds(intervalBetweenShots);
            }

        activeCor = null;
    }

    void ResetTrap(IGameEvent e)
    {
        enterEvent.gameObject.SetActive(true);
        bullets.ForEach((b) => { if (b != null) Destroy(b); });
        bullets.Clear();
        if (activeCor != null)
        {
            StopCoroutine(activeCor);
            activeCor = null;
        }
    }

    public void ReceiveDamage(float amount)
    {
        if (amount > 0)
            Destroy(gameObject);
    }

    public EObjectType GetObjectType()
    {
        return EObjectType.Enemy;
    }
}
