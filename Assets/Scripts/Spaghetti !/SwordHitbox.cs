using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SwordHitbox : NetworkBehaviour
{
    [Header("Hit Settings")]
    public float lifeTime = 0.35f;
    public float knockbackForce = 8f;
    public float knockbackMult = 1f;
    public LayerMask enemyLayers = ~0;
    public bool singleHit = true;

    private bool _hasHit;
    private Vector3 _hitDirection;
    public void ServerInit(Vector3 hitDirection, float force, float mult)
    {
        _hitDirection = hitDirection;
        knockbackForce = force;
        knockbackMult = mult;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            Invoke(nameof(ServerDespawn), lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bomb") { other.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * knockbackForce,ForceMode.Impulse); Debug.Log("BOMBHIT"); }

        var kb = other.GetComponent<EnemyKnockbackController>();

        if (kb == null) return;

        kb.ActivateKnockback(_hitDirection, knockbackForce, knockbackMult);


        
    }


    private void ServerDespawn()
    {
        if (!IsServer) return;
        var no = GetComponent<NetworkObject>();
        if (no != null && no.IsSpawned) no.Despawn(true);
        else Destroy(gameObject.transform.parent.gameObject);
    }
}