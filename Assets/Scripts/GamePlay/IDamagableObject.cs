public interface IDamagableObject 
{
    // Returns true if damage dealt was fatal
    bool DealDamage(float amount);

    bool IsAlive();

    EType GetType();
}

