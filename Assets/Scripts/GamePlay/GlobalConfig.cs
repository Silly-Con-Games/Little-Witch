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
}

