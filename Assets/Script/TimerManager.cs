using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;
    [Min(0f)] public float randomEventTimerMin = 10f;
    [Min(0f)] public float randomEventTimerMax = 50f;

    private readonly List<EntityRandomEvent> registeredEntities = new List<EntityRandomEvent>(64);

    private float nextBeatInSeconds;
    private float elapsed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ValidateTimers();
        ScheduleNextBeat();
    }

    private void FixedUpdate()
    {
        elapsed += Time.fixedDeltaTime;

        if (elapsed >= nextBeatInSeconds)
        {
            elapsed = 0f;
            OnBeat();
            ScheduleNextBeat();
        }
    }

    private void ValidateTimers()
    {
        if (randomEventTimerMin < 0f) randomEventTimerMin = 0f;
        if (randomEventTimerMax < randomEventTimerMin) randomEventTimerMax = randomEventTimerMin;
    }

    private void ScheduleNextBeat()
    {
        nextBeatInSeconds = Random.Range(randomEventTimerMin, randomEventTimerMax);
    }

    public void Register(EntityRandomEvent entity)
    {
        if (entity == null) return;
        if (registeredEntities.Contains(entity)) return;
        registeredEntities.Add(entity);
    }

    public void Unregister(EntityRandomEvent entity)
    {
        if (entity == null) return;
        registeredEntities.Remove(entity);
    }

    private void OnBeat()
    {
        for (int i = registeredEntities.Count - 1; i >= 0; i--)
        {
            EntityRandomEvent e = registeredEntities[i];

            if (e == null)
            {
                registeredEntities.RemoveAt(i);
                continue;
            }

            if (e.isActiveAndEnabled)
                e.TriggerRandomEvent();
        }
    }
}
