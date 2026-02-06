using Unity.Netcode;
using UnityEngine;

public class DamagePlayerOnContact : NetworkBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float knockBackForce = 5f;

    [Header("Refs")]
    [Tooltip("Racine du projectile (pour détruire l'ensemble après impact)")]
    [SerializeField] private GameObject parentRoot;

    private Rigidbody _rbCached;
    private ProjectilPlayer  _projectilPlayer;

    private void Awake()
    {
        _rbCached = GetComponent<Rigidbody>();
        _projectilPlayer = GetComponentInParent<ProjectilPlayer>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        PlayerMovement victimPM = other.GetComponent<PlayerMovement>();
        if (victimPM == null) return;

        if (_projectilPlayer == null) return;
        if (_projectilPlayer.OwnerPlayerId.Value == victimPM.currentId) return;

        Vector2 dirXZ = LastDirXZOrForward();
        
        victimPM.ApplyDamageServer(damage, knockBackForce, dirXZ);

        DestroyProjectile();
    }

    private Vector2 LastDirXZOrForward()
    {
        if (_rbCached != null)
        {

            Vector2 v = new Vector2(_rbCached.linearVelocity.x, _rbCached.linearVelocity.z);
            if (v.sqrMagnitude > 1e-6f) return v.normalized;
        }
        Vector3 fwd = transform.forward;
        return new Vector2(fwd.x, fwd.z).normalized;
    }

    private void DestroyProjectile()
    {
        var netObj = GetComponentInParent<NetworkObject>();
        if (netObj && netObj.IsSpawned)
            netObj.Despawn(true);

        if (parentRoot != null)
            Destroy(parentRoot);
    }
}
