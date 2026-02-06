using UnityEngine;

public abstract class EntityRandomEvent : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.Register(this);
    }

    protected virtual void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.Unregister(this);
    }

    // -------------------------
    // Events communs
    // -------------------------

    protected void CommonEvent01()
    {
        Debug.Log($"{gameObject.name} \"CommonEvent01 class EntityRandomEvent\"");
    }

    protected void CommonEvent02()
    {
        Debug.Log($"{gameObject.name} \"CommonEvent02 class EntityRandomEvent\"");
    }

    protected void CommonEvent03()
    {
        Debug.Log($"{gameObject.name} \"CommonEvent03 class EntityRandomEvent\"");
    }

    protected void TriggerRandomCommonEvent()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0: CommonEvent01(); break;
            case 1: CommonEvent02(); break;
            default: CommonEvent03(); break;
        }
    }
    public abstract void TriggerRandomEvent();
}
