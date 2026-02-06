using UnityEngine;

public class DecorRandomEvent : EntityRandomEvent
{
    private void DecorEvent01()
    {
        Debug.Log($"{gameObject.name} \"DecorEvent01 class DecorRandomEvent\"");
    }

    private void DecorEvent02()
    {
        Debug.Log($"{gameObject.name} \"DecorEvent02 class DecorRandomEvent\"");
    }

    private void TriggerRandomDecorEvent()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0) DecorEvent01();
        else DecorEvent02();
    }

    public override void TriggerRandomEvent()
    {
        // 50% commun / 50% spécifique
        if (Random.value < 0.5f)
            TriggerRandomCommonEvent();
        else
            TriggerRandomDecorEvent();
    }
}