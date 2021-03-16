public interface IDamagableObject 
{
    // Returns true if damage dealt was fatal
    void DealDamage(float amount);

    EType GetType();
}

