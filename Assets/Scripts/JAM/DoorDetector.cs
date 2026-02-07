using UnityEngine;

public class DoorDetector : MonoBehaviour
{
    public GameObject door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        door.SetActive(true);
        if(LevelsManager.Instance.lvlIndex>0) LevelsManager.Instance.Levels[LevelsManager.Instance.lvlIndex-1].shiningDoor.SetActive(false);
        Camera.main.GetComponent<LevelFollower>().MoveToIndex();
        PlayerClass.instance.PV = Mathf.Min(5,PlayerClass.instance.PV+1);
    }
}
