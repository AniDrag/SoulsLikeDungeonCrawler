using UnityEngine;
namespace AniDrag.WeaponPack
{
    /// <summary>
    /// Base class for all firing strategies (hitscan, projectile, charge, etc.).
    /// </summary>
    public abstract class FireStrategy : ScriptableObject
    {
        [Header("Strategy Info")] public string strategyName = "Default Strategy";

        /// <summary>
        /// Called when the weapon equips this strategy. Use to cache references or initialize.
        /// </summary>
        public virtual void Initialize(RangedWeapon weapon)
        {
        }

        /// <summary>
        /// Called every frame the fire button is held (or pressed, depending on weapon settings).
        /// </summary>
        public abstract void OnFire(RangedWeapon weapon, bool isPressed);

        /// <summary>
        /// Optional alt‑fire behavior.
        /// </summary>
        public virtual void OnAltFire(RangedWeapon weapon, bool isPressed)
        {
        }
    }
}