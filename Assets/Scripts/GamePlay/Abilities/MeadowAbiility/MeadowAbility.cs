using UnityEngine;
using Config;
using System.Collections.Generic;

[System.Serializable]
public class MeadowAbility : MainAbility
{
    public MAGrass grassPrefab;
    public MeadowAbilityConfig conf { get => internalConf; set { internalConf = value; mainAbilityConfig = value.baseConfig; } }

    private MeadowAbilityConfig internalConf;

    private HashSet<IDamagable> alreadyHit = new HashSet<IDamagable>();

    public override void CastAbility()  
    {
        base.CastAbility();
        Debug.Log("Casted meadow ability!");
        alreadyHit.Clear();

        var parent = new GameObject("MAParent").transform;
        parent.position = playerController.transform.position;
        parent.rotation = playerController.transform.rotation;

        int half = (conf.projectileCnt - 1) / 2;
        float stepWidth = conf.spellWidth / conf.projectileCnt; 

        for (int i = 0; i < conf.projectileCnt; ++i)
        {

            Vector2 randCircle = Random.insideUnitCircle.normalized;
            Vector3 randCircleV3 = new Vector3(randCircle.x, 0, -Mathf.Abs(randCircle.y));
            Vector3 startPoint1 = new Vector3(randCircleV3.x, -3, randCircleV3.z);
            Vector3 startPoint2 = new Vector3(randCircleV3.x, -2, randCircleV3.z);
            Vector3 heightingPoint = randCircleV3 * 1.5f;
            heightingPoint.y = Random.Range(0.5f, 1);

            Vector3 endPoint = new Vector3(stepWidth * i - half * stepWidth, 0.1f, 1 - Mathf.Abs(i- half) * 0.1f);
            Vector3 intermediete = endPoint + Vector3.forward * (conf.maxRange * 0.5f);
            Vector3 destPoint1 = endPoint + Vector3.forward * (conf.maxRange - endPoint.z);
            Vector3 destPoint2 = endPoint + Vector3.forward * (conf.maxRange * 1.5f);

            Vector3[] points = { startPoint1, startPoint2, heightingPoint, endPoint, intermediete, destPoint1, destPoint2};

            var inst = GameObject.Instantiate(grassPrefab, parent);
            inst.Init(new CatmulRollSpline(points), conf.speed, OnHit);
        }
        
    }

    private void OnHit(Collider other, MAGrass owner)
    {
        IDamagable dmg = other.gameObject.GetComponent<IDamagable>();
        if (dmg != null && dmg.GetObjectType() == EObjectType.Enemy && !alreadyHit.Contains(dmg))
        {
            dmg.ReceiveDamage(conf.damage);
            GameObject.Destroy(owner);
            alreadyHit.Add(dmg);
        }
    }

    public void SteppedOnMeadow()
    {
        playerController.ScaleSpeedModifier(conf.MSMultiplier);
    }

    public void SteppedFromMeadow()
    {
        playerController.ScaleSpeedModifier(1/conf.MSMultiplier);
    }
}
