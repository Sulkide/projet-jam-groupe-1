using UnityEngine;

public interface IKnockbackable
{
    void Knockback(Vector3 direction, float force, float mult = 1f);
}