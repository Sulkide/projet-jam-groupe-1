using UnityEngine;

public class PlayerRandomEvent : EntityRandomEvent
{
    private void PlayerEvent01()
    {
        Debug.Log($"{gameObject.name} \"PlayerEvent01 class PlayerRandomEvent\"");
    }

    private void PlayerEvent02()
    {
        Debug.Log($"{gameObject.name} \"PlayerEvent02 class PlayerRandomEvent\"");
    }

    private void TriggerRandomPlayerEvent()
    {
        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            PlayerEvent01();
        }
        else
        {
            PlayerEvent02();
        }
    }

    public override void TriggerRandomEvent()
    {
        if (Random.value < 0.5f)
        {
            TriggerRandomCommonEvent();
        }
        else
        {
            TriggerRandomPlayerEvent();
        }
    }
}