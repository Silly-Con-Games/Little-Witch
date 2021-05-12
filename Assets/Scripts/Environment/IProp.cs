using UnityEngine;

public interface IProp : IObjectType
{
    void Spawn(bool immediate);
    void Despawn(bool immediate);
    void Die(bool immediate);
    void Revive(bool immediate);
    BiomeType GetBiomeType();
}
