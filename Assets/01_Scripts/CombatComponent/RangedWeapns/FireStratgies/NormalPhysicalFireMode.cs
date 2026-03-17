using AniDrag.Core;
using UnityEngine;


namespace AniDrag.WeaponPack
{
    [CreateAssetMenu(menuName = "Weapon/Strategies/Physical Projectile")]
    public class NormalPhysicalFireMode : FireStrategy
    {
        [Header("Projectile Settings")]
        public int baseDamage = 10;
        public StatType damageStatMultiplier = StatType.DEX;
        public float projectileSpeed = 50f;

        public override void OnFire(RangedWeapon weapon, bool isPressed)
        {
            if (!isPressed) return;
            if (!weapon.CanFire()) return;

            weapon.ConsumeAmmo();
            weapon.SetNextFireTime(weapon.fireRate);

            for (int i = 0; i < weapon.projectilesPerShot; i++)
            {
                SpawnProjectile(weapon);
            }
        }

        private void SpawnProjectile(RangedWeapon weapon)
        {
            if (weapon.projectilePrefab == null || weapon.FirePoint == null) return;

            // Determine direction with spread
            Vector3 direction = weapon.FirePoint.forward;
            if (weapon.spreadAngle > 0)
            {
                float randomX = Random.Range(-weapon.spreadAngle, weapon.spreadAngle);
                float randomY = Random.Range(-weapon.spreadAngle, weapon.spreadAngle);
                Quaternion spread = Quaternion.Euler(randomY, randomX, 0);
                direction = spread * direction;
            }

            GameObject proj = Instantiate(weapon.projectilePrefab, weapon.FirePoint.position, Quaternion.LookRotation(direction));
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = direction * projectileSpeed;

            Projectile projScript = proj.GetComponent<Projectile>();
            if (projScript != null)
            {
                // Calculate final damage
                int finalDamage = baseDamage;

                if (weapon.Owner != null)
                {
                    var statsProvider = weapon.Owner.GetComponent<IStatsProvider>();
                    if (statsProvider != null)
                    {
                        Stats totalStats = statsProvider.GetTotalStats();
                        switch (damageStatMultiplier)
                        {
                            case StatType.STR:
                                finalDamage += totalStats.STR * 2;
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

                projScript.damage = finalDamage;
                projScript.Initialize(weapon.Owner);
            }
        }
    }
}