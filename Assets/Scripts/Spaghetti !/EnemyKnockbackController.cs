using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class EnemyKnockbackController : MonoBehaviour
{
    [Header("Disable While Knockback")]
    [Tooltip("Scripts à désactiver pendant le knockback. Si vide et AutoCollect=true, on auto-remplit.")]
    public List<MonoBehaviour> scriptsToDisable = new List<MonoBehaviour>();

    [Tooltip("Si la liste est vide, récupère automatiquement les scripts à désactiver sur l'ennemi.")]
    [SerializeField] private bool autoCollectIfEmpty = true;

    [Tooltip("Inclure aussi les scripts sur les enfants (utile si ton AI est sur un enfant).")]
    [SerializeField] private bool includeChildren = false;

    [Tooltip("Optionnel : désactiver le NavMeshAgent si présent.")]
    [SerializeField] private bool disableNavMeshAgent = true;

    [Header("Stop Detection")]
    [SerializeField] private float stopSpeedThreshold = 0.15f;
    [SerializeField] private float stopConfirmTime = 0.10f;
    [SerializeField] private float maxActiveTime = 1.5f;

    [Header("Knockback")]
    [SerializeField] private bool resetHorizontalVelocity = true;
    [SerializeField] private float clampHorizontalSpeed = 14f;

    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private bool _running;
    private Coroutine _routine;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();

        // Auto-collect si demandé
        if (autoCollectIfEmpty && (scriptsToDisable == null || scriptsToDisable.Count == 0))
            AutoCollectScripts();
    }

    private void AutoCollectScripts()
    {
        scriptsToDisable = scriptsToDisable ?? new List<MonoBehaviour>();
        scriptsToDisable.Clear();

        MonoBehaviour[] all = includeChildren
            ? GetComponentsInChildren<MonoBehaviour>(true)
            : GetComponents<MonoBehaviour>();

        for (int i = 0; i < all.Length; i++)
        {
            var b = all[i];
            if (b == null) continue;

            // Ne pas se désactiver soi-même
            if (b == this) continue;

            // Optionnel : ne pas toucher au NavMeshAgent ici (on le gère à part)
            if (b is NavMeshAgent) continue;

            // Ne pas désactiver les colliders/renderers par défaut (sinon tu perds les triggers)
            if (b is Collider) continue;
            if (b is Renderer) continue;

            scriptsToDisable.Add(b);
        }

        Debug.Log($"[EnemyKnockbackController] AutoCollect => {scriptsToDisable.Count} scripts sur {name}");
    }

    /// <summary>Appelé par l'épée au contact.</summary>
    public void ActivateKnockback(Vector3 direction, float force, float mult = 1f)
    {
  
        
        Debug.Log($"uiruhfioeshufquivçàifoju_bçàvioqkijubçài)ofpfijuçàb)ofpsdijuvà)ofp^qisdfpà");
        

        direction.y = 0f;
        direction.Normalize();
        DisableStuff();
        ApplyImpulse(direction, force, mult);
        RestartRoutine();

        if (autoCollectIfEmpty && (scriptsToDisable == null || scriptsToDisable.Count == 0))
            AutoCollectScripts();

        _running = true;


    }

    private void RestartRoutine()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(WaitUntilStoppedThenRestore());
    }

    public void DisableStuff()
    {
        Debug.Log($"[EnemyKnockbackController] DisableStuff count={scriptsToDisable.Count} on {name}");

        for (int i = 0; i < scriptsToDisable.Count; i++)
        {
            var b = scriptsToDisable[i];
            Debug.Log($"[EnemyKnockbackController] disabling[{i}] => {(b ? b.GetType().Name : "NULL")}");
            if (b != null) b.enabled = false;
        }

        if (disableNavMeshAgent && _agent != null)
            _agent.enabled = false;
    }

    private void RestoreStuff()
    {
        for (int i = 0; i < scriptsToDisable.Count; i++)
        {
            var b = scriptsToDisable[i];
            if (b != null) b.enabled = true;
        }

        if (disableNavMeshAgent && _agent != null)
            _agent.enabled = true;
    }

    private void ApplyImpulse(Vector3 dir, float force, float mult)
    {
     

        _rb.AddForce(dir * (force * mult), ForceMode.Impulse);

        if (clampHorizontalSpeed > 0f)
        {
            Vector3 hv = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (hv.magnitude > clampHorizontalSpeed)
            {
                hv = hv.normalized * clampHorizontalSpeed;
                _rb.linearVelocity = new Vector3(hv.x, _rb.linearVelocity.y, hv.z);
            }
        }
    }

    private IEnumerator WaitUntilStoppedThenRestore()
    {
        float underThresholdTimer = 0f;
        float t0 = Time.time;

        while (true)
        {
            if (Time.time - t0 >= maxActiveTime) break;
            if (_rb == null) break;

            Vector3 hv = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            if (hv.magnitude <= stopSpeedThreshold)
            {
                underThresholdTimer += Time.deltaTime;
                if (underThresholdTimer >= stopConfirmTime) break;
            }
            else
            {
                underThresholdTimer = 0f;
            }

            yield return null;
        }

        RestoreStuff();

        _running = false;
        _routine = null;
    }
}
