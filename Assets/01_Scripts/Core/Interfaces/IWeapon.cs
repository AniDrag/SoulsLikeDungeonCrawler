using UnityEngine;

namespace AniDrag.Core
{
    public interface IWeapon
    {
        GameObject Owner { get; set; }
        // General 
        void Equip();
        void Unequip();
        void Attack(bool isPressed);
        void AltAttack(bool isPressed);
        
        // Melee
        void Block(bool isPressed);
        
        //Ranged
        void Aim(bool isPressed);
        void Reload(bool isPressed); // For ranged weapons
        
        // AI helpers mostly
        bool CanAttack();      // true if weapon is ready to attack
        bool IsAttacking();    // true while attack animation is playing
        float GetAttackRange(); // optional, for AI positioning
    }
}