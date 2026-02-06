using UnityEngine;

public class EnemyRandomEvent : EntityRandomEvent
{
    private void EnemyEvent01()
    {
        Debug.Log($"{gameObject.name} \"EnemyEvent01 class EnemyRandomEvent\"");
    }

    private void EnemyEvent02()
    {
        Debug.Log($"{gameObject.name} \"EnemyEvent02 class EnemyRandomEvent\"");
    }

    private void TriggerRandomEnemyEvent()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0) EnemyEvent01();
        else EnemyEvent02();
    }

    public override void TriggerRandomEvent()
    {
        // 50% commun / 50% spécifique
        if (Random.value < 0.5f)
            TriggerRandomCommonEvent();
        else
            TriggerRandomEnemyEvent();
    }
}