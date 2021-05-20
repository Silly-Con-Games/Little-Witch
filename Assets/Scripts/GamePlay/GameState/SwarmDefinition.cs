using System;

[Serializable]
public class SwarmDefinition 
{
    public EnemiesSpawn spawnPoint;
    public int MeleeCnt;
    public int RangedCnt;
    public int EnvDestroyerCnt;
    public int BomberCnt;

    public void Spawn()
    {
        spawnPoint.Spawn(EnemyType.Melee, MeleeCnt);
        spawnPoint.Spawn(EnemyType.Ranged, RangedCnt);
        spawnPoint.Spawn(EnemyType.EnvDestroyer, EnvDestroyerCnt);
        spawnPoint.Spawn(EnemyType.Bomber, BomberCnt);
    }
}
