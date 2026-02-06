using UnityEngine;

public class EnemyDamageBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
		if (other.tag == "Player")
		{
			PlayerClass p = other.GetComponent<PlayerClass>();

			p.TakeDamage(1);
		}
	}
}
