using UnityEngine;

public class LevelFollower : MonoBehaviour
{

    Vector3 Offset, target;
    private void Start()
    {
        Offset = transform.position - LevelsManager.Instance.Levels[0].transform.position;
        MoveToIndex();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime*2);
    }
    public void MoveToIndex()
    {
        target = LevelsManager.Instance.Levels[LevelsManager.Instance.lvlIndex].transform.position + Offset;
    }
}
