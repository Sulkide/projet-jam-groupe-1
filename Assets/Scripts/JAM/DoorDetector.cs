using UnityEngine;

public class DoorDetector : MonoBehaviour
{
    public GameObject door;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        door.SetActive(true);
    }
}
