using UnityEngine;

public abstract class AICore_TargetingStrategy : ScriptableObject
{
    /// <summary> Called when the strategy is assigned to an AI (after instantiation). </summary>
    public virtual void Initialize(AICore_Controller ai) { }

    /// <summary> Called every frame by the AI controller. Should update selectedTarget. </summary>
    public abstract Transform UpdateTarget(AICore_Controller ai);
}