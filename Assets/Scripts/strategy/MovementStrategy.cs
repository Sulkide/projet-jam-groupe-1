using UnityEngine;

public abstract class MovementStrategy : ScriptableObject
{
    public abstract MovementMode Mode { get; }
    // Tick "temps réel" (appelé depuis Update/FixedUpdate) ou depuis un TurnManager (tick par tour)
    public abstract void Tick(BaseEntity entity, float deltaTime);
}