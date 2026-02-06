using UnityEngine;

public class BombScript : MonoBehaviour
{
    public float timer = 2f;
    public GameObject bombHitZone;
    void Update()
    {
        timer-= Time.deltaTime;
        if(timer <= 0)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Enemy")
        {
            Explode();
        }
    }

    public void Explode()
    {
        bombHitZone.SetActive(true);
    }
}
