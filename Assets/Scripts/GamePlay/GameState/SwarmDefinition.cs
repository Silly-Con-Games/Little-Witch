using System;

[Serializable]
public class SwarmDefinition 
{
    public EnemiesSpawn spawnPoint;
    public int MeleeCnt;
    public int MeleeMiniBossCnt;
    public int RangedCnt;
    public int RangedMinibossCnt;
    public int EnvDestroyerCnt;
    public int BomberCnt;

    public void Spawn()
    {
        spawnPoint.Spawn(EnemyType.Melee, MeleeCnt);
        spawnPoint.Spawn(EnemyType.Ranged, RangedCnt);
        spawnPoint.Spawn(EnemyType.EnvDestroyer, EnvDestroyerCnt);
        spawnPoint.Spawn(EnemyType.Bomber, BomberCnt);
        spawnPoint.Spawn(EnemyType.RangedMiniboss, RangedMinibossCnt);
        spawnPoint.Spawn(EnemyType.MeleeMiniboss, MeleeMiniBossCnt);
    }
}
