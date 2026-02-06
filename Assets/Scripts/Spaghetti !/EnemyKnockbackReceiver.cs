using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyKnockbackReceiver : NetworkBehaviour, IKnockbackable
{
    [Header("Knockback")]
    public float maxHorizontalSpeedAfterHit = 12f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Knockback(Vector3 direction, float force, float mult = 1f)
    {
        if (!IsServer) return; 

        direction.y = 0f;
        if (direction.sqrMagnitude < 1e-6f) return;

        var v = _rb.linearVelocity;
        v.y = _rb.linearVelocity.y;
        v.x = 0f;
        v.z = 0f;
        _rb.linearVelocity = v;

        Vector3 impulse = direction.normalized * (force * mult);
        _rb.AddForce(impulse, ForceMode.Impulse);

   
        Vector3 hv = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        if (hv.magnitude > maxHorizontalSpeedAfterHit)
        {
            hv = hv.normalized * maxHorizontalSpeedAfterHit;
            _rb.linearVelocity = new Vector3(hv.x, _rb.linearVelocity.y, hv.z);
        }
    }
}