using AniDrag.Core;
using UnityEngine;
namespace AniDrag.WeaponPack
{
    [CreateAssetMenu(menuName = "Weapon/Strategies/Hitscan")]
    public class Hitscan : FireStrategy
    {
        [Header("Hitscan Settings")]
        public int baseDamage = 10;
        public StatType damageStatMultiplier = StatType.DEX; // e.g., DEX increases damage

        public override void OnFire(RangedWeapon weapon, bool isPressed)
        {
            if (!isPressed) return;
            if (!weapon.CanFire()) return;

            weapon.ConsumeAmmo();
            weapon.SetNextFireTime(weapon.fireRate);

            for (int i = 0; i < weapon.projectilesPerShot; i++)
            {
                PerformHit(weapon);
            }
        }

        private void PerformHit(RangedWeapon weapon)
        {
            // Calculate direction with spread
            Vector3 direction = weapon.FirePoint.forward;
            if (weapon.spreadAngle > 0)
            {
                float randomX = Random.Range(-weapon.spreadAngle, weapon.spreadAngle);
                float randomY = Random.Range(-weapon.spreadAngle, weapon.spreadAngle);
                Quaternion spread = Quaternion.Euler(randomY, randomX, 0);
                direction = spread * direction;
            }

            Ray ray = new Ray(weapon.FirePoint.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, weapon.raycastRange, weapon.hitLayers))
            {
                // Calculate final damage
                int finalDamage = baseDamage;

                // Apply stat multiplier if owner has stats
                if (weapon.Owner != null)
                {
                    var statsProvider = weapon.Owner.GetComponent<IStatsProvider>();
                    if (statsProvider != null)
                    {
                        Stats totalStats = statsProvider.GetTotalStats();
                        switch (damageStatMultiplier)
                        {
                            case StatType.STR:
                                finalDamage += totalStats.STR * 2; // example scaling
                                break;
                            case StatType.DEX:
                                finalDamage += totalStats.DEX * 2;
                                break;
                            case StatType.INT:
                                finalDamage += totalStats.INT * 2;
                                break;
                        }
                    }
                }

                var damagable = hit.collider.GetComponent<IDamagable>();
                damagable?.TakeDamage(finalDamage, weapon.Owner);
            }
        }
    }
}
