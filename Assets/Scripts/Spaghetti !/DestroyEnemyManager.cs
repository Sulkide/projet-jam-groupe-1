using System.Collections.Generic;
using UnityEngine;

public class DestroyEnemyManager : MonoBehaviour , IBeatListener
{
    [Header("Pool")]
    public List<GameObject> destroyEnemyObjects = new List<GameObject>();
    public bool autoFindInChildren = true;

    [Header("Activation Rules")]
    [Min(1)] public int activeCount = 2;
    public float switchInterval = 3f;
    public bool disableAllEachStep = true;

    [Header("Selection")]
    public bool shuffleOrder = true;

    public int seed = 0;

    private float _timer;

    private readonly List<int> _order = new List<int>();
    private int _cursor;

    private void Awake()
    {
        if (autoFindInChildren)
        {
            destroyEnemyObjects.Clear();
            var traps = GetComponentsInChildren<DestroyEnemy>(true);
            for (int i = 0; i < traps.Length; i++)
                destroyEnemyObjects.Add(traps[i].gameObject);
        }

        BuildOrder();
        StepActivation();
    }

    private void Update()
    {
        if (switchInterval <= 0f) return;

        _timer += Time.deltaTime;
        if (_timer >= switchInterval)
        {
            _timer = 0f;
            StepActivation();
        }
    }
    public void OnBeat()
    {
        StepActivation();
    }

    public void StepActivation()
    {
        int count = destroyEnemyObjects.Count;
        if (count == 0) return;

        int n = Mathf.Clamp(activeCount, 1, count);

        if (disableAllEachStep)
        {
            for (int i = 0; i < count; i++)
                if (destroyEnemyObjects[i]) destroyEnemyObjects[i].SetActive(false);
        }

        if (_order.Count != count) BuildOrder();
        if (_cursor >= _order.Count)
        {
            BuildOrder();
            _cursor = 0;
        }
        
        for (int k = 0; k < n; k++)
        {
            int idxInOrder = (_cursor + k) % _order.Count;
            int objIndex = _order[idxInOrder];

            var go = destroyEnemyObjects[objIndex];
            if (go) go.SetActive(true);
        }

        _cursor += n;
    }

    private void BuildOrder()
    {
        _order.Clear();

        int count = destroyEnemyObjects.Count;
        for (int i = 0; i < count; i++)
            _order.Add(i);

        if (!shuffleOrder) return;

        var rng = (seed == 0) ? new System.Random() : new System.Random(seed);
        
        for (int i = _order.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (_order[i], _order[j]) = (_order[j], _order[i]);
        }
    }
    public void DisableAll()
    {
        for (int i = 0; i < destroyEnemyObjects.Count; i++)
            if (destroyEnemyObjects[i]) destroyEnemyObjects[i].SetActive(false);
    }
}
