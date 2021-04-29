using UnityEngine;
using System;
using Config;

[Serializable]
public class ForestAbility : MainAbility
{
    public FARootPath rootPathPrefab;

    public FARootCircle rootCirclePrefab;

    public ForestAbilityConfig conf { get => internalConf; set { internalConf = value; mainAbilityConfig = value.baseConfig; } }

    private ForestAbilityConfig internalConf;
    private EObjectType target = EObjectType.Enemy;

    private Vector3 destination;

    public override void CastAbility()
    {
        base.CastAbility();

        Vector3 spawnpoint = playerController.transform.position;
        spawnpoint.y = 0;
        destination = playerController.mouseWorldPosition;
        destination.y = 0;

        Vector3 direction = destination - spawnpoint;
        float distance = Math.Min(direction.magnitude, conf.maxRange);
        if(distance == 0)
            direction = Vector3.forward;
        else
            direction /= distance;

        destination = spawnpoint + direction * distance;


        float pathDistance = distance - conf.rootBurstRadius/2;
        if (pathDistance  > 0)
        {
            var inst = GameObject.Instantiate(rootPathPrefab);
            inst.transform.position = spawnpoint;

            inst.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

            inst.Init(pathDistance - conf.rootBurstRadius/2, pathDistance / conf.rootPathSpeed, OnRootPathHit, OnRootPathEnd);
        }
        else
        {
            OnRootPathEnd();
        }


        Debug.Log("Casted forest ability!");
    }

    private void OnRootPathHit(Collider other)
    {
        IDamagable enemy = other.GetComponent<IDamagable>();
        if (enemy != null && enemy.GetObjectType() == target)
            enemy.ReceiveDamage(conf.rootPathDamage);
    }

    private void OnRootPathEnd()
    {
        var inst = GameObject.Instantiate(rootCirclePrefab);
        inst.transform.position = destination;
        inst.Init(conf.rootBurstRadius, conf.rootBurstDensity, OnRootCircleHit);
    }

    private void OnRootCircleHit(Collider other)
    {
        IDamagable dmgEnemy = other.GetComponent<IDamagable>();
        if (dmgEnemy != null && dmgEnemy.GetObjectType() == target)
            dmgEnemy.ReceiveDamage(conf.rootPathDamage);

        IRootable rootEnemy = other.GetComponent<IRootable>();
        if (rootEnemy != null && rootEnemy.GetObjectType() == target)
            rootEnemy.ReceiveRoot(conf.rootDuration);
    }
}
