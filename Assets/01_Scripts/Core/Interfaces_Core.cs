using System;
using UnityEngine;

namespace AniDrag.Core
{
    #region General Interfaces
    public interface IPredicate
    {
        bool Evaluate();
    }
    #endregion

    #region Entity / inventory focused
    /// <summary>
    /// Objects that can take damage (e.g., characters, destructibles).
    /// </summary>
    public interface IDamagable
    {
        public void TakeDamage(int amount);

        public Action DeathEvent { get; set; }
    }

    /// <summary>
    /// Objects that can receive consumable effects (e.g., healing, buffs).
    /// </summary>
    public interface IEffectReceiver
    {
        void ApplyEffect(ConsumableEffectType effectType, int amount, float duration);
    }

    /// <summary>
    /// Items that can be equipped (weapons, armor, etc.).
    /// </summary>
    public interface IEquippable
    {
        EquipmentType EquipmentType { get; }
        GameObject WorldPrefab { get; }
        // Add any other properties needed for equipment (e.g., stats)
    }

    /// <summary>
    /// Objects that can equip items (players, NPCs).
    /// </summary>
    public interface IEquipmentUser
    {
        void Equip(IEquippable item);
        void Unequip(EquipmentType slot);
        IEquippable GetEquipped(EquipmentType slot);
    }
    #endregion

    #region Animation Interfaces

    public interface IState
    {
        string StateName();
        void TransitionSetup();
        void OnEnter();
        void OnUpdate();
        void OnFixedUpdate();
        void OnTickUpdate();
        void OnExit();
    }

    public interface ITransition
    {
        public IState to { get; }
        public IPredicate condition { get; }

    }


    #endregion
}