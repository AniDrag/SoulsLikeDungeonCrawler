using System.Collections;
using AniDrag.Core;
using UnityEngine;

namespace AniDrag.WeaponPack
{
   public class RangedWeapon : WeaponCore
    {
        [Header("Animation")]
        [SerializeField] private Animator weaponAnimator;
        [SerializeField] private string fireTrigger = "Fire";
        [SerializeField] private string aimBool = "Aiming";
        [SerializeField] private string reloadTrigger = "Reload";

        [Header("Firing Modes")]
        public FireStrategy primaryFireStrategy;
        public FireStrategy altFireStrategy;

        [Header("General")]
        public float fireRate = 0.2f;               // seconds between shots
        public int projectilesPerShot = 1;
        public float spreadAngle = 0f;
        public LayerMask hitLayers;                  // for hitscan
        public float raycastRange = 100f;            // for hitscan
        public GameObject projectilePrefab;          // for physical projectiles

        [Header("Damage")]
        public int baseDamage = 10;                   // base damage, can be overridden by strategies

        [Header("Ammo")]
        public int magazineSize = 30;
        public int maxAmmo = 90;
        public bool infiniteAmmo = false;
        public float reloadTime = 2f;

        [Header("References")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject muzzleFlashPrefab;
        [SerializeField] private AudioClip fireSound;
        [SerializeField] private AudioClip reloadSound;

        // Public properties for strategies
        public Transform FirePoint => firePoint;
        public int CurrentAmmoInMag { get; private set; }
        public int TotalRemainingAmmo { get; private set; }
        public bool IsReloading { get; private set; }

        private float nextFireTime = 0f;

        private void Awake()
        {
            CurrentAmmoInMag = magazineSize;
            TotalRemainingAmmo = maxAmmo - magazineSize;
            primaryFireStrategy?.Initialize(this);
            altFireStrategy?.Initialize(this);
        }

        #region IWeapon Implementation

        public override void Attack(bool isPressed)
        {
            if (IsReloading) return;
            primaryFireStrategy?.OnFire(this, isPressed);
        }

        public override void AltAttack(bool isPressed)
        {
            if (IsReloading) return;
            altFireStrategy?.OnAltFire(this, isPressed);
        }

        public override void Block(bool isPressed)
        {
            // Used for aiming
            if (weaponAnimator != null)
                weaponAnimator.SetBool(aimBool, isPressed);
        }

        public override void Reload(bool isPressed)
        {
            if (!isPressed || IsReloading) return;
            if (CurrentAmmoInMag >= magazineSize || (!infiniteAmmo && TotalRemainingAmmo <= 0)) return;

            StartCoroutine(ReloadRoutine());
        }

        public override void Equip()
        {
            base.Equip();
            gameObject.SetActive(true);
        }

        public override void Unequip()
        {
            base.Unequip();
            gameObject.SetActive(false);
        }

        #endregion

        private IEnumerator ReloadRoutine()
        {
            IsReloading = true;
            if (reloadSound != null && firePoint != null)
                AudioSource.PlayClipAtPoint(reloadSound, firePoint.position);

            if (weaponAnimator != null)
                weaponAnimator.SetTrigger(reloadTrigger);

            yield return new WaitForSeconds(reloadTime);

            if (infiniteAmmo)
            {
                CurrentAmmoInMag = magazineSize;
            }
            else
            {
                int needed = magazineSize - CurrentAmmoInMag;
                int taken = Mathf.Min(needed, TotalRemainingAmmo);
                CurrentAmmoInMag += taken;
                TotalRemainingAmmo -= taken;
            }

            IsReloading = false;
        }

        // Helper methods for strategies
        public bool CanFire()
        {
            return !IsReloading && Time.time >= nextFireTime && (infiniteAmmo || CurrentAmmoInMag > 0);
        }

        public void ConsumeAmmo()
        {
            if (!infiniteAmmo)
                CurrentAmmoInMag--;
        }

        public void SetNextFireTime(float delay)
        {
            nextFireTime = Time.time + delay;
        }
    }
}