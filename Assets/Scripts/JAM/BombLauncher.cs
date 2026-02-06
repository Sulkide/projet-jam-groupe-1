using UnityEngine;

public class BombLauncher : MonoBehaviour
{
    float CoolDown =1f;

    public GameObject bombPrefab;

    float bombSpawnDistance = 2f;

    void Update()
    {
        CoolDown-=Time.deltaTime;
        if(CoolDown <= 0) { SpawnBomb(); CoolDown = 5f; }
    }

    void SpawnBomb()
    {
        Transform player = PlayerClass.instance.transform;
        Vector3 dir = player.position - transform.position;
        GameObject bomb = Instantiate(bombPrefab, transform.position + dir.normalized * bombSpawnDistance, Quaternion.identity);
        bomb.GetComponent<Rigidbody>().AddForce(dir.normalized*50f, ForceMode.Impulse);
    }
}
