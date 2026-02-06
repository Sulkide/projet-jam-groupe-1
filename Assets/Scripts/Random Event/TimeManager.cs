using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Random beat interval (seconds)")]
    [Min(0.01f)] public float randomEventTimerMin = 2f;
    [Min(0.01f)] public float randomEventTimerMax = 5f;

    [Header("Start")]
    public bool playOnStart = true;
    public float startDelay = 0f;

    [Header("Listeners (optional)")]
    [SerializeField] private List<MonoBehaviour> listenersInInspector = new();

    private readonly List<IBeatListener> _listeners = new();
    private float _timer;
    private float _nextBeatTime;
    private bool _isPlaying;

    public int BeatCount { get; private set; }

    private void Awake()
    {
        // Charge les listeners assignés dans l’inspector (sans FindObject)
        _listeners.Clear();
        for (int i = 0; i < listenersInInspector.Count; i++)
        {
            var mb = listenersInInspector[i];
            if (mb is IBeatListener listener)
                _listeners.Add(listener);
        }

        ValidateMinMax();
    }

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    private void FixedUpdate()
    {
        if (!_isPlaying) return;

        _timer += Time.fixedDeltaTime;

        if (_timer >= _nextBeatTime)
        {
            TriggerBeat();
            ScheduleNextBeat();
        }
    }

    public void Play()
    {
        ValidateMinMax();
        _isPlaying = true;
        BeatCount = 0;
        _timer = 0f;

        // Premier beat
        _nextBeatTime = Mathf.Max(0f, startDelay);
        if (_nextBeatTime <= 0f)
        {
            // Option: déclencher direct au lancement si startDelay = 0
            TriggerBeat();
            ScheduleNextBeat();
        }
    }

    public void Stop()
    {
        _isPlaying = false;
    }

    public void Register(IBeatListener listener)
    {
        if (listener == null) return;
        if (_listeners.Contains(listener)) return;
        _listeners.Add(listener);
    }

    public void Unregister(IBeatListener listener)
    {
        if (listener == null) return;
        _listeners.Remove(listener);
    }

    private void TriggerBeat()
    {
        BeatCount++;

        // On déclenche tout le monde au même “battement”
        for (int i = 0; i < _listeners.Count; i++)
            _listeners[i]?.OnBeat();
    }

    private void ScheduleNextBeat()
    {
        float dt = Random.Range(randomEventTimerMin, randomEventTimerMax);
        _nextBeatTime = _timer + dt;
    }

    private void ValidateMinMax()
    {
        if (randomEventTimerMax < randomEventTimerMin)
            (randomEventTimerMin, randomEventTimerMax) = (randomEventTimerMax, randomEventTimerMin);

        randomEventTimerMin = Mathf.Max(0.01f, randomEventTimerMin);
        randomEventTimerMax = Mathf.Max(0.01f, randomEventTimerMax);
    }
}
