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
        public SecondaryAbConfig secondaryAbility;
    }

    [Serializable]
    public struct SecondaryAbConfig
    {
        public float attacksPerSecond;
        public float damage;
        public float projectileSpeed;
        public float maxRange;
    }
}

