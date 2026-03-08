using AniDrag.CharacterComponents;
using AniDrag.Core;
using System.Collections;
using UnityEngine;

namespace AniDrag.WeaponPack
{
    public class RangedWeapon : WeaponCore
    {
        [Header("========================\n" +
                "    Projectile details      \n" +
                "========================")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [Tooltip("Offset applied when instantiating projectile (e.g., to fix model misalignment)")]
        [field: SerializeField] public Vector3 offsetOnInstatioation = Vector3.zero;

        [Header("========================\n" +
                "    Weapon Stats      \n" +
                "========================")]
        [SerializeField] private float projectileLaunchForce = 20f;
        [SerializeField] private float fireChargeTime = 0f;
        [SerializeField] private float fireRate = 5f;
        [SerializeField] private float burstDelay = 0.1f;
        [SerializeField, Range(1, 10)] private int projectilesPerShot = 1;
        [SerializeField] private float spreadAngle = 2f;
        [SerializeField] private int shotsPerMagazine = 30;
        [SerializeField] private int magazineCapacity = 5;
        [SerializeField] private float reloadTime = 2f;
        [SerializeField] private bool infiniteAmmo = false;

        // Optional: if assigned, used for player aiming
        [Tooltip("If assigned, this camera's forward direction is used for aiming (for player). Otherwise uses owner's forward.")]
        [SerializeField] private Camera playerCamera;

        // For AI: we can set a target to aim at
        private Entity currentTarget;  // if null, uses forward direction

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
            totalRemainingAmmo = shotsPerMagazine * (magazineCapacity - 1);
        }

        #region Public API for AI

        /// <summary>
        /// Set a target for the weapon to aim at. If null, weapon uses owner's forward direction.
        /// </summary>
        public void SetTarget(Entity target)
        {
            currentTarget = target;
        }

        /// <summary>
        /// Convenience method for AI: sets target and fires.
        /// </summary>
        public void FireAtTarget(Entity target)
        {
            SetTarget(target);
            Fire(true);
        }

        #endregion

        #region WeaponCore Overrides

        public override void Fire(bool isPressed)
        {
            if (!isPressed)
            {
                if (isCharging)
                {
                    float chargeDuration = Time.time - chargeStartTime;
                    if (chargeDuration >= fireChargeTime)
                        AttemptFire();
                    isCharging = false;
                }
                return;
            }

            if (fireChargeTime > 0f)
            {
                isCharging = true;
                chargeStartTime = Time.time;
            }
            else
            {
                AttemptFire();
            }
        }

        public override void AltFire(bool isPressed)
        {
            // Optional: alt-fire logic
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
            // Optional: aim down sights
        }

        #endregion

        #region Firing Logic

        private void AttemptFire()
        {
            if (Time.time < nextFireTime) return;
            if (currentAmmoInMag <= 0 && !infiniteAmmo)
            {
                Debug.Log("Out of ammo! Press reload.");
                return;
            }

            if (!infiniteAmmo) currentAmmoInMag--;

            if (fireCoroutine != null) StopCoroutine(fireCoroutine);
            fireCoroutine = StartCoroutine(FireProjectiles());

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

            // Apply spread
            Vector3 finalDirection = ApplySpread(aimDirection);

            // Instantiate projectile
            GameObject proj = Instantiate(projectilePrefab, firePoint.position + offsetOnInstatioation, Quaternion.LookRotation(finalDirection));
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = finalDirection * projectileLaunchForce;

            Projectile projectileScript = proj.GetComponent<Projectile>();
            if (projectileScript != null)
                projectileScript.Initialize(owner);
        }

        /// <summary>
        /// Returns the direction to aim.
        /// Priority: currentTarget (if set) → playerCamera (if assigned) → owner's forward.
        /// Also applies eye‑height offset if shooting at a target.
        /// </summary>
        private Vector3 GetAimDirection()
        {
            // If we have a target, aim at its position (with optional height offset)
            if (currentTarget != null)
            {
                // Use owner's eye position (height 1.6f) as requested
                Vector3 eyePos = owner != null ? owner.transform.position + Vector3.up * 1.6f : firePoint.position;
                Vector3 targetPos = currentTarget.transform.position + Vector3.up * 1.6f; // aim at eye level
                return (targetPos - eyePos).normalized;
            }

            // Player aiming via camera
            if (playerCamera != null)
                return playerCamera.transform.forward;

            // Fallback to owner's forward
            if (owner != null)
                return owner.transform.forward;

            // Ultimate fallback
            return firePoint.forward;
        }

        private Vector3 ApplySpread(Vector3 baseDirection)
        {
            if (projectilesPerShot <= 1 || spreadAngle <= 0)
                return baseDirection;

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

        #region Ammo Helpers

        public void AddAmmo(int amount) => totalRemainingAmmo += amount;
        public int GetCurrentMagazineAmmo() => currentAmmoInMag;
        public int GetTotalRemainingAmmo() => totalRemainingAmmo;

        #endregion
    }
}