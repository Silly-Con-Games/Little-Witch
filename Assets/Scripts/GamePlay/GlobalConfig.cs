using System;

namespace Config
{
    [Serializable]
    public struct GlobalConfig
    {
        public WitchConfig witchConfig;
        public GlobalEnemyConfig globalEnemyConfig;
        public float respawnTime;
    }

    [Serializable]
    public struct WitchConfig
    {
        public float health;
        public float energyMax;
        public float energyInitial;
        public float movementSpeed;
        public float jumpHeight;
        public MeleeAbilityConfig meeleeAbility;
        public ChargeAbilityConfig chargeAbility;
        public ForestAbilityConfig forestAbility;
        public WaterAbilityConfig waterAbility;
        public MeadowAbilityConfig meadowAbility;

    }

    [Serializable]
    public struct MeleeAbilityConfig
    {
        public float cooldown;
        public float damage;
    }

    [Serializable]
    public struct ChargeAbilityConfig
    {
        public float damageMin;
        public float damageMax;
        public float distanceMin;
        public float distanceMax;
        public float explosionRadiusMin;
        public float explosionRadiusMax;
        public float explosionSpeedMin;
        public float explosionSpeedMax;
        public float spawnRadiusMin;
        public float spawnRadiusMax;
        public float speedMin;
        public float speedMax;
        public float chargeTimeMax;
        public float cooldown;
        public float witchSlowMSMultiplier;
        public float energyCost;
    }

    [Serializable]
    public struct MainAbilityConfig
    {
        public float cooldown;
    }

    [Serializable]
    public struct ForestAbilityConfig 
    {
        public MainAbilityConfig baseConfig;
        public float rootPathDamage;
        public float rootBurstDamage;
        public float maxRange;
        public float rootPathSpeed;
        public float rootDuration;
        public float rootBurstRadius;
        public int rootBurstDensity;
    }

    [Serializable]
    public struct WaterAbilityConfig
    {
        public MainAbilityConfig baseConfig;
        public float healPerSec;
    }

    [Serializable]
    public struct MeadowAbilityConfig
    {
        public MainAbilityConfig baseConfig;
        public float MSMultiplier;
    }

    [Serializable]
    public struct EnemyConfig
    {
        public float idleDuration;
        public float chasingDuration;
        
        // min and max value for the range the enemy can go from roam position
        public float moveRangeMin;
        public float moveRangeMax;
        //--------------------------------------------------------------------
        
        // range after reaching which enemy starts chasing player(switches state to CHASE)
        public float maxRangeToPlayer;

        // range after reaching which enemy starts attacking player(switches state to ATTACK)
        public float attackRange;
        public float speed;
        public float attackCooldown;
        public float healthPoints;
        
        // default duration for slow and root effect
        public float slowDefault;
        public float rootDefault;
    }

    [Serializable]
    public struct EnemyRangedConfig
    {
        public EnemyConfig baseConfig;
    }

    [Serializable]
    public struct EnemyMeleeConfig
    {
        public EnemyConfig baseConfig;
        // delay before dashing
        public float dashDelay;
        public float dashDuration;
        // dash speed modifier (speed will be multiplied by this value)
        public float dashSpeedModifier;
        // range after reaching which enemy starts dashing
        public float dashRange;
    }

    [Serializable]
    public struct EnemyBomberConfig
    {
        public EnemyConfig baseConfig;
    }

    [Serializable]
    public struct EnemyEnvDestroyerConfig
    {
        public EnemyConfig baseConfig;
        // max number of melee/ranged enemies guards
        public int enemiesMeleeMax;
        public int enemiesRangedMax;
    }

    [Serializable]
    public struct MineConfig
    {
        public float explosionDelay;
        public float damageRange;
        public float baseDamage;
        public float disappearingDuration;
    }

    [Serializable]
    public struct GlobalEnemyConfig
    {
        public EnemyRangedConfig enemyRangedConfig;
        public EnemyMeleeConfig enemyMeleeConfig;
        public EnemyBomberConfig enemyBomberConfig;
        public EnemyEnvDestroyerConfig enemyEnvDestroyerConfig;
        public MineConfig mineConfig;
    }

}

