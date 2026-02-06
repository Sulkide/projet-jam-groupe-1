using System.Collections.Generic;
using UnityEngine;

public class EntityRandomEvent : MonoBehaviour, IBeatListener
{
    public enum CommonEvent
    {
        ShakeEntity,
        ChangeScale
    }

    [Header("Event selection")]
    public bool enableShakeEntity = true;
    public bool enableChangeScale = true;

    [Header("Behavior")]
    [Tooltip("Si true, évite de rejouer 2 fois de suite le même event (si possible).")]
    public bool avoidRepeat = true;

    private readonly List<CommonEvent> _pool = new List<CommonEvent>(8);
    private CommonEvent? _lastEvent = null;

    public void OnBeat()
    {
        RebuildPool();

        if (_pool.Count == 0)
        {
            Debug.LogWarning($"{name} EntityRandomEvent: aucun event activé.");
            return;
        }

        CommonEvent chosen = PickRandomEvent();
        PlayEvent(chosen);
        _lastEvent = chosen;
    }

    private void RebuildPool()
    {
        _pool.Clear();

        if (enableShakeEntity)  _pool.Add(CommonEvent.ShakeEntity);
        if (enableChangeScale)  _pool.Add(CommonEvent.ChangeScale);
    }

    private CommonEvent PickRandomEvent()
    {
        if (!avoidRepeat || _pool.Count <= 1 || _lastEvent == null)
            return _pool[Random.Range(0, _pool.Count)];

        // Eviter la répétition immédiate si possible
        CommonEvent candidate;
        int safety = 8;

        do
        {
            candidate = _pool[Random.Range(0, _pool.Count)];
            safety--;
        }
        while (candidate.Equals(_lastEvent.Value) && safety > 0);

        return candidate;
    }

    private void PlayEvent(CommonEvent ev)
    {
        switch (ev)
        {
            case CommonEvent.ShakeEntity:
                ShakeEntity();
                break;

            case CommonEvent.ChangeScale:
                ChangeScale();
                break;
        }
    }

    protected virtual void ShakeEntity()
    {
        Debug.Log($"{name} -> ShakeEntity (TODO: implémentation plus tard)");
    }

    protected virtual void ChangeScale()
    {
        Debug.Log($"{name} -> ChangeScale (TODO: implémentation plus tard)");
    }
}
