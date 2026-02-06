using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string displayName = "Entity";
    private EntityType entityType = EntityType.Pnj;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float speed = 2.5f;
    [SerializeField] private MovementStrategy movementStrategy;

    [Header("Runtime State")]
    [SerializeField] private Direction desiredDirection = Direction.Still;
    [SerializeField] private State state = State.Idle;
    private Direction _lastNonStillDir = Direction.Right;

    [Header("Sensing")]
    [Tooltip("Rayon du capteur joueur (m).")]
    [SerializeField, Min(0f)] private float playerSenseRadius = 2.0f;
    [SerializeField] private LayerMask playerMask = ~0;
    [SerializeField] private bool debugSensing = true;

    [Header("Control Locks")]
    [SerializeField] private float movementLockedUntil;

    private Rigidbody _rb;
    private EntityRole _role;

    private readonly HashSet<int> _playersInside = new HashSet<int>();

    public string ID => id;
    public string DisplayName => displayName;
    public EntityType Type => entityType;
    public float Speed => speed;
    public Direction DesiredDirection => desiredDirection;
    public State CurrentState => state;
    public MovementStrategy Strategy => movementStrategy;
    public EntityRole Role => _role;
    public bool IsMovementLocked => Time.time < movementLockedUntil;

    public void SetDisplayName(string name) => displayName = name;
    public void SetID(string newId) => id = newId;
    public void SetType(EntityType t) => entityType = t;
    public void SetSpeed(float newSpeed) => speed = Mathf.Max(0f, newSpeed);
    public void SetStrategy(MovementStrategy strategy) => movementStrategy = strategy;

    public void LockMovement(float seconds)
    {
        movementLockedUntil = Mathf.Max(movementLockedUntil, Time.time + Mathf.Max(0f, seconds));
    }

    public void SetDesired(Direction dir, State sIfMoving)
    {
        if (dir == Direction.Still)
        {
            desiredDirection = Direction.Still;
            state = State.Idle;
            return;
        }

        desiredDirection = dir;
        _lastNonStillDir = dir;
        state = sIfMoving;
    }

    public void SetState(State s) => state = s;

    public void StopMoving()
    {
        desiredDirection = Direction.Still;
        state = State.Idle;
    }

    private void Reset()
    {
        playerMask = LayerMask.GetMask("Player");
    }

    private void OnValidate()
    {
        var roles = GetComponents<EntityRole>();
        if (roles.Length > 1)
            Debug.LogWarning($"[BaseEntity] {name}: Plusieurs rôles trouvés ({roles.Length}). Garde seulement un script de rôle par entité.");
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
            _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _role = GetComponent<EntityRole>();
        if (_role != null)
        {
            _role.Initialize(this);
            SetType(_role.RoleType);
        }
    }

    private void Update()
    {
        // Décision / sensing en Update, OK
        SensePlayers();
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // IMPORTANT : si lock, on n'appelle PAS l'IA de mouvement (sinon elle écrase le knockback)
        if (IsMovementLocked)
        {
            StopMoving();
            return;
        }

        // IMPORTANT : role d'abord (peut changer de stratégie), puis stratégie
        if (_role != null)
            _role.Tick(dt);

        if (movementStrategy != null)
            movementStrategy.Tick(this, dt);
    }

    public void MoveInDirection(Direction d, float moveSpeed, float dt)
    {
        if (_rb == null) return;

        Vector3 dir = DirectionUtil.ToVector(d);
        Vector3 delta = dir * moveSpeed * dt;
        _rb.MovePosition(_rb.position + delta);
    }

    private void SensePlayers()
    {
        if (playerSenseRadius <= 0f) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, playerSenseRadius, playerMask, QueryTriggerInteraction.Collide);

        if (debugSensing)
        {
         
        }
    }
}
