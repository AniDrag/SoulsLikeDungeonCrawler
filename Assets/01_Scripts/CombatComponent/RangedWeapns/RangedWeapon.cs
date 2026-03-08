using AniDrag.Core;
using System.Collections;
using UnityEngine;

namespace AniDrag.WeaponPack
{
    /// <summary>
    /// Handles most ranged weapon types: bows, guns, magic staffs, etc.
    /// Supports physical projectiles, hitscan, charging, burst fire, ammo, and reloading.
    /// </summary>
    public class RangedWeapon : WeaponCore
    {
        [Header("========================\n" +
                "Ranged weapon specific ref\n" +
                "========================")]
        [SerializeField] private Camera playerCamera;          // For aiming / raycast direction

        [Header("========================\n" +
                "    Projectile details      \n" +
                "========================")]
        [SerializeField] private GameObject projectilePrefab;  // Physical projectile (null if hitscan)
        [SerializeField] private Transform firePoint;          // Where projectiles spawn / raycast starts
        [Tooltip("Offset applied when instantiating projectile (e.g., to fix model misalignment)")]
        [field: SerializeField] public Vector3 offsetOnInstatioation = Vector3.zero;

        [Header("========================\n" +
                "    Weapon Stats      \n" +
                "========================")]
        [SerializeField] private float projectileLaunchForce = 20f;   // Speed for physical projectiles
        [SerializeField] private float fireChargeTime = 0f;           // Time to hold before firing (0 = instant)
        [SerializeField] private float fireRate = 5f;                  // Shots per second (auto weapons)
        [SerializeField] private float burstDelay = 0.1f;              // Delay between multiple projectiles in one shot
        [SerializeField, Range(1, 10)] private int projectilesPerShot = 1;  // Simultaneous projectiles (spread)
        [SerializeField] private float spreadAngle = 2f;                // Spread in degrees for multi-projectile
        [SerializeField] private int shotsPerMagazine = 30;            // Number of shots per magazine
        [SerializeField] private int magazineCapacity = 5;              // Number of magazines (total ammo)
        [SerializeField] private float reloadTime = 2f;                 // Time to reload
        [SerializeField] private bool hitScan = false;                  // True = raycast, false = physical projectile
        [SerializeField] private bool infiniteAmmo = false;             // Ignores ammo consumption
        [SerializeField] private LayerMask hitLayers = -1;              // What hitscan can hit

        // Private state
        private int currentAmmoInMag;        // Rounds left in current magazine
        private int totalRemainingAmmo;       // Rounds not in magazine (reserve)
        private bool isFiring = false;
        private bool isCharging = false;
        private float chargeStartTime;
        private float nextFireTime;            // For fire rate limiting
        private Coroutine reloadCoroutine;
        private Coroutine fireCoroutine;

        private void Awake()
        {
            inputType = WeaponInputType.Ranged;
            InitializeAmmo();
        }

        private void InitializeAmmo()
        {
            currentAmmoInMag = shotsPerMagazine;
            totalRemainingAmmo = shotsPerMagazine * (magazineCapacity - 1); // one mag already loaded
        }

        #region WeaponCore Overrides

        public override void Fire(bool isPressed)
        {
            if (!isPressed)
            {
                // Button released � handle charge fire
                if (isCharging)
                {
                    float chargeDuration = Time.time - chargeStartTime;
                    if (chargeDuration >= fireChargeTime)
                        AttemptFire();
                    isCharging = false;
                }
                return;
            }

            // Button pressed
            if (fireChargeTime > 0f)
            {
                // Start charging
                isCharging = true;
                chargeStartTime = Time.time;
            }
            else
            {
                // Instant fire
                AttemptFire();
            }
        }

        public override void AltFire(bool isPressed)
        {
            // Optional: alt-fire logic (e.g., scope, underbarrel, etc.)
            Debug.Log("AltFire not implemented");
        }

        public override void Reload(bool isPressed)
        {
            if (!isPressed) return;
            if (currentAmmoInMag >= shotsPerMagazine || totalRemainingAmmo <= 0) return;
            if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
        }

        public override void Aim(bool isPressed)
        {
            // Optional: aim down sights (ADS) � can be used to zoom or change weapon state
            Debug.Log("Aim not implemented");
        }

        #endregion

        #region Firing Logic

        private void AttemptFire()
        {
            if (Time.time < nextFireTime) return;                     // Fire rate limit
            if (currentAmmoInMag <= 0 && !infiniteAmmo)
            {
                Debug.Log("Out of ammo! Press reload.");
                return;
            }

            // Consume ammo (if not infinite)
            if (!infiniteAmmo) currentAmmoInMag--;

            // Start firing sequence (could be burst or single)
            if (fireCoroutine != null) StopCoroutine(fireCoroutine);
            fireCoroutine = StartCoroutine(FireProjectiles());

            // Set next allowed fire time
            nextFireTime = Time.time + (1f / fireRate);
        }

        private IEnumerator FireProjectiles()
        {
            for (int i = 0; i < projectilesPerShot; i++)
            {
                if (hitScan)
                    PerformHitscan();
                else
                    SpawnPhysicalProjectile();

                if (i < projectilesPerShot - 1)
                    yield return new WaitForSeconds(burstDelay);
            }
        }

        private void SpawnPhysicalProjectile()
        {
            if (projectilePrefab == null || firePoint == null)
            {
                Debug.LogError("Physical projectile requires a prefab and firePoint!");
                return;
            }

            // Calculate direction with spread
            Vector3 direction = GetSpreadDirection();

            // Instantiate projectile
            GameObject proj = Instantiate(projectilePrefab, firePoint.position + offsetOnInstatioation, Quaternion.LookRotation(direction));
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileLaunchForce;
            }

            // Optional: pass damage / owner info via a component on the projectile
            var projDamage = proj.GetComponent<IDamageDealer>();
            if (projDamage != null)
                projDamage.Initialize(/* damage, owner */);
        }

        private void PerformHitscan()
        {
            if (playerCamera == null)
            {
                Debug.LogError("Hitscan requires a player camera!");
                return;
            }

            // Direction with spread
            Vector3 direction = GetSpreadDirection();

            Ray ray = new Ray(playerCamera.transform.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hitLayers))
            {
                // Apply damage
                var damageable = hit.collider.GetComponent<IDamagable>();
                damageable?.TakeDamage(/* damage value from somewhere */);

                // Optionally spawn impact effect
            }
        }

        private Vector3 GetSpreadDirection()
        {
            if (playerCamera == null) return firePoint.forward;

            Vector3 baseDirection = playerCamera.transform.forward;

            if (projectilesPerShot > 1 && spreadAngle > 0)
            {
                // Generate random spread within cone
                float randomX = Random.Range(-spreadAngle, spreadAngle);
                float randomY = Random.Range(-spreadAngle, spreadAngle);
                Quaternion spreadRot = Quaternion.Euler(randomY, randomX, 0);
                baseDirection = spreadRot * baseDirection;
            }

            return baseDirection.normalized;
        }

        #endregion

        #region Reloading

        private IEnumerator ReloadCoroutine()
        {
            // Play reload animation/sound here
            yield return new WaitForSeconds(reloadTime);

            int needed = shotsPerMagazine - currentAmmoInMag;
            int available = Mathf.Min(needed, totalRemainingAmmo);
            currentAmmoInMag += available;
            totalRemainingAmmo -= available;

            Debug.Log($"Reloaded. Ammo: {currentAmmoInMag}/{totalRemainingAmmo + currentAmmoInMag}");
        }

        #endregion

        #region Ammo Helpers (optional)

        public void AddAmmo(int amount)
        {
            totalRemainingAmmo += amount;
        }

        public int GetCurrentMagazineAmmo() => currentAmmoInMag;
        public int GetTotalRemainingAmmo() => totalRemainingAmmo;

        #endregion
    }
}