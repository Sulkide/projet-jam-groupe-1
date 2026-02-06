using UnityEngine;

public class DestroyEnemyManagerBeatAdapter : MonoBehaviour, IBeatListener
{
    [SerializeField] private DestroyEnemyManager destroyEnemyManager;

    public void OnBeat()
    {
        if (destroyEnemyManager != null)
            destroyEnemyManager.OnBeat();
    }
}