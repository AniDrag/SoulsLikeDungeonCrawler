using UnityEngine;
using AniDrag.WeaponPack;

namespace AniDrag.AI
{
    /// <summary>
    /// Handles weapon usage and attack cooldowns for the AI.
    /// Listens to animation events for hit detection.
    /// </summary>
    public class AICombat : MonoBehaviour
    {
        private WeaponCore currentWeapon;
        private float lastAttackTime;

        /// <summary>
        /// Initialize with the equipped weapon.
        /// Call this when the enemy spawns or equips a new weapon.
        /// </summary>
        public void SetWeapon(WeaponCore weapon)
        {
            currentWeapon = weapon;
        }

        /// <summary>
        /// Check if the AI can attack based on cooldown.
        /// </summary>
        public bool CanAttack(float cooldown)
        {
            return Time.time - lastAttackTime >= cooldown;
        }

        /// <summary>
        /// Perform an attack using the current weapon.
        /// Uses the weapon's inputType to decide between Attack() and Fire().
        /// </summary>
        public void PerformAttack()
        {
            if (currentWeapon == null) return;

            // Use the weapon's input type to call the correct primary attack method
            switch (currentWeapon.inputType)
            {
                case WeaponInputType.Melee:
                    currentWeapon.Attack(true);
                    break;
                case WeaponInputType.Ranged:
                    currentWeapon.Fire(true);
                    break;
            }

            lastAttackTime = Time.time;
        }
    }
}