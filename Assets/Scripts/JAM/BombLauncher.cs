using UnityEngine;

public class BombLauncher : MonoBehaviour
{
    float CoolDown =1f;

    public GameObject bombPrefab;

    float bombSpawnDistance = 3f;

    int index;

    private void Awake()
    {
        //LevelClass lvl = transform.parent.GetComponent<LevelClass>();
		//index = LevelsManager.Instance.Levels.IndexOf( lvl );
    }
    void Update()
    {
        //if (LevelsManager.Instance.lvlIndex != index) return;
        CoolDown-=Time.deltaTime;
        if(CoolDown <= 0) { SpawnBomb(); CoolDown = 4f;}
    }

    void SpawnBomb()
    {
        if (Vector3.Distance(transform.position, PlayerClass.instance.transform.position) > 20f) return;
        Debug.Log("here");
        GetComponent<EnemyRole>()._anim.SetTrigger("Attack");
        Transform player = PlayerClass.instance.transform;
        Vector3 dir = player.position - transform.position;
        GameObject bomb = Instantiate(bombPrefab, transform.position + dir.normalized * bombSpawnDistance, Quaternion.identity);
        bomb.GetComponent<Rigidbody>().AddForce(dir.normalized*50f*dir.magnitude/30f, ForceMode.Impulse);
    }
}
