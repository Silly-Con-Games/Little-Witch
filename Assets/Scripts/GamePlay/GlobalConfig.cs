using System;

namespace Config
{
    [Serializable]
    public struct GlobalConfig
    {
        public WitchConfig witchConfig;
    }

    [Serializable]
    public struct WitchConfig
    {
        public float health;
        public float movementSpeed;
        public float jumpHeight;
        public MeleeAbilityConfig meeleeAbility;
        public ChargeAbilityConfig chargeAbility;
        public ForestAbilityConfig forestAbility;
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
        public float witchSlow;
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
    public struct EnemyRangedConfig
    {
    }

    [Serializable]
    public struct EnemyMeleeConfig
    {
    }

    [Serializable]
    public struct EnemyBomberConfig
    {
    }

    [Serializable]
    public struct EnemyEnvDestroyerConfig
    {
    }

}

