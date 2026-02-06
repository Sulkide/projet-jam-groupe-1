using System.Collections;
using UnityEngine;

public class BombHitZone : MonoBehaviour
{

    public float strenght = 1f;

    private void Awake()
    {
        StartCoroutine(Explosion());
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.GetComponent<Rigidbody>().AddForce((other.transform.position - transform.position).normalized * strenght, ForceMode.Impulse);
            StartCoroutine(Explosion());
        }

        if(other.tag == "Player")
        {
            other.GetComponent<PlayerClass>().TakeDamage(1);
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(transform.parent.gameObject);
    }
}
