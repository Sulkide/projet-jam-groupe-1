using UnityEngine;

[DisallowMultipleComponent]
public class DestroyEnemy : MonoBehaviour
{
    [Header("Detection")]
    public bool useTrigger = true;
    public LayerMask enemyLayers = ~0;
    public bool requireEnemyTag = false;
    public string enemyTag = "Enemy";
    public bool destroyRootObject = true;
    public bool disableAfterHit = false;

    private bool _consumed;

    private void OnTriggerEnter(Collider other)
    {
        if (!useTrigger) return;
        TryDestroy(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (useTrigger) return;
        TryDestroy(collision.gameObject);
    }

    private void TryDestroy(GameObject hit)
    {
        if (_consumed) return;
        if (!hit) return;
        
        //int layer = hit.layer;
        //if (((1 << layer) & enemyLayers.value) == 0) return;

        //if (requireEnemyTag && !hit.CompareTag(enemyTag)) return;

        GameObject target = destroyRootObject ? hit.transform.root.gameObject : hit;

        if (target == gameObject) return;

        if(target.tag == "Enemy")
        {
            target.GetComponent<EnemyRole>().StartCoroutine("Death");
        }
        else
        {
			Destroy(target);
		}

		if (disableAfterHit)
        {
            _consumed = true;
            gameObject.SetActive(false);
        }
    }
}