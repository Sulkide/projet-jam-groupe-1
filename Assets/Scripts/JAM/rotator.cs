using UnityEngine;

public class rotator : MonoBehaviour
{
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }
    void Update()
    {
        Vector3 vec = cam.transform.position - transform.position;

        transform.rotation = Quaternion.LookRotation(vec);
        transform.rotation = Quaternion.Euler(transform.rotation.x+45, 0,transform.rotation.z);
        Debug.DrawRay(transform.position, vec);
    }
}
