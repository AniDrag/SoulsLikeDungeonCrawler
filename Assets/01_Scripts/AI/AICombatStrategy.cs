using AniDrag.AI;
using AniDrag.CharacterComponents;
using UnityEngine;

/// <summary>
/// Base class for combat strategies. Defines how the AI attacks.
/// </summary>
public abstract class AICombatStrategy : ScriptableObject
{
    
    public abstract float GetAttackRange(AIController ai);

    public abstract float GetCooldown(AIController ai);

    public virtual void Initialize(AIController ai) { }

    public abstract void Attack(AIController ai, Entity target);
}