using System.Collections.Generic;
using UnityEngine;

public class LevelClass : MonoBehaviour
{

	public int EnemyAmount;

    public GameObject ExitDoor;
	public GameObject shiningDoor;

	bool ded = false;
    private void Update()
    {
        if (EnemyAmount <= 0&&!ded)
        {
            ded = true;
            LevelsManager.Instance.NextLevel();
            ExitDoor.SetActive(false);
            shiningDoor.SetActive(true);
        }
    }
}
