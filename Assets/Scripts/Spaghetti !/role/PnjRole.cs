using UnityEngine;

public class PnjRole : EntityRole
{
    public override EntityType RoleType => EntityType.Pnj;

    [Header("PNJ Params")]
    public float talkRadius = 3f;

    public override void Tick(float dt)
    {
        // PNJ : comportement très simple (garde la stratégie de base)
        // La Move Strategy (ex: Free) gère déjà son errance ; ici on pourrait bloquer le Run, etc.
    }
}