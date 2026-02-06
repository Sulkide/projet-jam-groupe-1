using UnityEngine;

public class LevelFollower : MonoBehaviour
{

    Vector3 Offset;
    private void Start()
    {
        Offset = transform.position - LevelsManager.Instance.Levels[0].transform.position;
    }

    public void MoveToIndex()
    {
        transform.position = LevelsManager.Instance.Levels[LevelsManager.Instance.lvlIndex].transform.position + Offset;
    }
}
