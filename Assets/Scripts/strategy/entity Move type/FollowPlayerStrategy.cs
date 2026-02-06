using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FollowPlayerStrategy", menuName = "NPC/Movement/Follow Player")]
public class FollowPlayerStrategy : MovementStrategy
{
    public override MovementMode Mode => MovementMode.Follow;

    [Header("Follow")]
    [Tooltip("Distance à partir de laquelle l'ennemi s'arrête (sur XZ).")]
    public float stopDistance = 1.1f;

    [Tooltip("Multiplicateur de vitesse pendant la poursuite.")]
    public float chaseSpeedMult = 1.3f;

    [Tooltip("Si true, la stratégie met l'entity en Idle quand elle est assez proche.")]
    public bool idleWhenClose = true;

    // Runtime par entité
    private readonly Dictionary<int, Transform> _targets = new Dictionary<int, Transform>();

    public void SetTarget(BaseEntity entity, Transform target)
    {
        if (entity == null) return;
        int key = entity.GetInstanceID();
        if (target == null) _targets.Remove(key);
        else _targets[key] = target;
    }

    public void ClearTarget(BaseEntity entity)
    {
        if (entity == null) return;
        _targets.Remove(entity.GetInstanceID());
    }

    public override void Tick(BaseEntity entity, float dt)
    {
        if (entity == null) return;

        if (!_targets.TryGetValue(entity.GetInstanceID(), out var target) || target == null)
        {
            entity.SetDesired(Direction.Still, State.Idle);
            entity.SetState(State.Idle);
            return;
        }

        Vector3 toTarget = target.position - entity.transform.position;
        toTarget.y = 0f;

        float dist = toTarget.magnitude;
        if (dist <= stopDistance)
        {
            if (idleWhenClose)
            {
                entity.SetDesired(Direction.Still, State.Idle);
                entity.SetState(State.Idle);
            }
            return;
        }

        Direction dir = ClosestDirection(toTarget);
        entity.SetDesired(dir, State.Walk);

        float s = entity.Speed * chaseSpeedMult;
        entity.MoveInDirection(dir, s, dt);
        entity.SetState(State.Walk);
    }

    private Direction ClosestDirection(Vector3 v)
    {
        v.y = 0f;
        if (v.sqrMagnitude < 0.0001f) return Direction.Still;

        v.Normalize();

        Direction best = Direction.Right;
        float bestDot = -Mathf.Infinity;

        for (int i = 1; i <= 8; i++)
        {
            var d = (Direction)i;
            var dv = DirectionUtil.ToVector(d);
            float dot = Vector3.Dot(v, dv);
            if (dot > bestDot)
            {
                bestDot = dot;
                best = d;
            }
        }
        return best;
    }
}
