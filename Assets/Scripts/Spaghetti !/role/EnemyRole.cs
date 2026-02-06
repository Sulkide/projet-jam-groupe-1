using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyRole : EntityRole, IKnockbackable
{
    public override EntityType RoleType => EntityType.Enemy;

    [Header("Detection")]
    public float aggroRadius = 8f;
    public float loseAggroRadius = 10f;

    [Tooltip("Layer(s) des joueurs (ex: 'Player').")]
    public LayerMask playerMask = ~0;

    [Header("Strategies")]
    [Tooltip("Stratégie par défaut quand aucune cible n'est détectée.")]
    public MovementStrategy freeMoveStrategy;

    [Tooltip("Stratégie de poursuite (ScriptableObject FollowPlayerStrategy).")]
    public FollowPlayerStrategy followStrategy;

    [Header("Stun / Knockback")]
    [Tooltip("Durée de stun appliquée par défaut lors d'un knockback.")]
    public float defaultStunSeconds = 0.25f;

    [Tooltip("Optionnel : clamp vitesse horizontale après knockback.")]
    public float knockbackClampSpeed = 14f;

    [Tooltip("Si true, reset la vitesse horizontale avant d'appliquer l'impulsion.")]
    public bool resetHorizontalVelocityOnKnockback = true;

    [Header("Runtime")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private float stunnedUntil;

    private Rigidbody _rb;
    private Animator _anim;

    // Buffer non-alloc pour éviter du GC
    private readonly Collider[] _hits = new Collider[16];

    public bool IsStunned => Time.time < stunnedUntil;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
    }

    private void OnDestroy()
    {
        LevelsManager.Instance.Levels[LevelsManager.Instance.lvlIndex].EnemyAmount--;
    }
    public override void Tick(float dt)
    {
        // 0) Si stunned : on coupe le déplacement AI (sinon la stratégie annule la physique)
        if (IsStunned)
        {
            if (entity != null)
            {
                entity.SetDesired(Direction.Still, State.Idle);
                entity.SetState(State.Idle);
            }
            return;
        }

        // 1) Gérer acquisition/perte de target
        UpdateTarget();

        // 2) Switch stratégie proprement
        if (currentTarget != null && followStrategy != null)
        {
            followStrategy.SetTarget(entity, currentTarget);

            if (entity.Strategy != followStrategy)
            { 
                entity.SetStrategy(followStrategy); 
                _anim.SetTrigger("Trigger");
			}
		}
        else
        {
            // Pas de target : revenir en free move
            if (followStrategy != null)
                followStrategy.ClearTarget(entity);

            if (freeMoveStrategy != null && entity.Strategy != freeMoveStrategy)
            {
                entity.SetStrategy(freeMoveStrategy);
                _anim.SetTrigger("Idle");
            }

		}

		// NOTE : ne pas bouger ici.
		// Le mouvement se fait dans MovementStrategy.Tick via BaseEntity.Update().
		// (Sinon tu risques un double déplacement.)
	}

    private void UpdateTarget()
    {
        if (entity == null) return;

        if (currentTarget != null)
        {
            Vector3 delta = currentTarget.position - entity.transform.position;
            delta.y = 0f;

            if (delta.magnitude > loseAggroRadius)
                currentTarget = null;

            return;
        }

        int count = Physics.OverlapSphereNonAlloc(
            entity.transform.position,
            aggroRadius,
            _hits,
            playerMask,
            QueryTriggerInteraction.Collide
        );

        float bestSqr = float.PositiveInfinity;
        Transform best = null;

        for (int i = 0; i < count; i++)
        {
            var c = _hits[i];
            if (!c) continue;

            Vector3 delta = c.transform.position - entity.transform.position;
            delta.y = 0f;

            float sqr = delta.sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = c.transform;
            }
        }

        currentTarget = best;
    }

    public void Stun(float seconds)
    {
        float end = Time.time + Mathf.Max(0f, seconds);
        if (end > stunnedUntil) stunnedUntil = end;
    }
    
    public void Knockback(Vector3 direction, float force, float mult = 1f)
    {
        print("evdfsjhifqhuiofhjiofjdsilvjwdklvujdsiovujdsiovjdsiovjkdsklvjdsioi");
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_rb == null) return;

        direction.y = 0f;
        if (direction.sqrMagnitude < 1e-6f) return;
        
        Stun(defaultStunSeconds);
        
        

        Vector3 impulse = direction.normalized * (force * mult);

        if (entity != null)
            entity.LockMovement(defaultStunSeconds);

        
        var v = _rb.linearVelocity;
        v.x = 0f; v.z = 0f;
        _rb.linearVelocity = v;
        entity.LockMovement(0.25f); 

        _rb.AddForce(direction.normalized * (force * mult), ForceMode.Impulse);

        if (knockbackClampSpeed > 0f)
        {
            Vector3 hv = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (hv.magnitude > knockbackClampSpeed)
            {
                hv = hv.normalized * knockbackClampSpeed;
                _rb.linearVelocity = new Vector3(hv.x, _rb.linearVelocity.y, hv.z);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!entity) return;

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.15f);
        Gizmos.DrawSphere(entity.transform.position, aggroRadius);

        Gizmos.color = new Color(1f, 0.8f, 0.1f, 0.10f);
        Gizmos.DrawSphere(entity.transform.position, loseAggroRadius);
    }

    public IEnumerator Death()
    {
        _anim.SetTrigger("Death");
        _rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

}
