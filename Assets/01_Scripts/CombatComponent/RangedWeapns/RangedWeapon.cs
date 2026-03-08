using AniDrag.Core;
using System.Collections;
using UnityEngine;

namespace AniDrag.WeaponPack
{
    /// <summary>
    /// Handles ranged weapons that fire physical projectiles (e.g., guns, bows, magic staffs).
    /// Supports charging, burst fire, ammo, reloading, and spread.
    /// </summary>
    public class RangedWeapon : WeaponCore
    {
        [Header("========================\n" +
                "    Projectile details      \n" +
                "========================")]
        [SerializeField] private GameObject projectilePrefab;          // Physical projectile prefab
        [SerializeField] private Transform firePoint;                  // Where projectiles spawn
        [Tooltip("Offset applied when instantiating projectile (e.g., to fix model misalignment)")]
        [field: SerializeField] public Vector3 offsetOnInstatioation = Vector3.zero;

        [Header("========================\n" +
                "    Weapon Stats      \n" +
                "========================")]
        [SerializeField] private float projectileLaunchForce = 20f;    // Speed of projectile
        [SerializeField] private float fireChargeTime = 0f;            // Time to hold before firing (0 = instant)
        [SerializeField] private float fireRate = 5f;                  // Shots per second (auto weapons)
        [SerializeField] private float burstDelay = 0.1f;              // Delay between multiple projectiles in one shot
        [SerializeField, Range(1, 10)] private int projectilesPerShot = 1;
        [SerializeField] private float spreadAngle = 2f;               // Spread in degrees for multi-projectile
        [SerializeField] private int shotsPerMagazine = 30;
        [SerializeField] private int magazineCapacity = 5;
        [SerializeField] private float reloadTime = 2f;
        [SerializeField] private bool infiniteAmmo = false;

        // Optional: if you want to use a camera for player aiming (if not assigned, falls back to owner's forward)
        [Tooltip("If assigned, this camera's forward direction is used for aiming (for player). Otherwise uses owner's forward.")]
        [SerializeField] private Camera playerCamera;

        // Private state
        private int currentAmmoInMag;
        private int totalRemainingAmmo;
        private bool isCharging = false;
        private float chargeStartTime;
        private float nextFireTime;
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
                // Button released – handle charge fire
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
            // Optional: aim down sights – can be used to zoom or change weapon state
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

            // Start firing sequence (burst)
            if (fireCoroutine != null) StopCoroutine(fireCoroutine);
            fireCoroutine = StartCoroutine(FireProjectiles());

            // Set next allowed fire time
            nextFireTime = Time.time + (1f / fireRate);
        }

        private IEnumerator FireProjectiles()
        {
            for (int i = 0; i < projectilesPerShot; i++)
            {
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

            // Determine aim direction
            Vector3 aimDirection = GetAimDirection();

            // Apply spread if needed
            Vector3 finalDirection = ApplySpread(aimDirection);

            // Instantiate projectile
            GameObject proj = Instantiate(projectilePrefab, firePoint.position + offsetOnInstatioation, Quaternion.LookRotation(finalDirection));
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = finalDirection * projectileLaunchForce;
            }

            // Set owner for damage attribution and XP
            Projectile projectileScript = proj.GetComponent<Projectile>();
            if (projectileScript != null)
                projectileScript.Initialize(owner); // owner is set by WeaponsController
        }

        /// <summary>
        /// Returns the direction the entity is looking.
        /// Uses playerCamera if assigned, otherwise owner's forward.
        /// </summary>
        private Vector3 GetAimDirection()
        {
            if (playerCamera != null)
                return playerCamera.transform.forward;

            if (owner != null)
                return owner.transform.forward;

            // Fallback to firePoint's forward (should not happen)
            return firePoint.forward;
        }

        private Vector3 ApplySpread(Vector3 baseDirection)
        {
            if (projectilesPerShot <= 1 || spreadAngle <= 0)
                return baseDirection;

            // Generate random spread within cone
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);
            Quaternion spreadRot = Quaternion.Euler(randomY, randomX, 0);
            return spreadRot * baseDirection;
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