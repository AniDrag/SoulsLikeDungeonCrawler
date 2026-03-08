using UnityEngine;

namespace AniDrag.WeaponPack
{

    public class RangedHitscanWeapon : WeaponCore
    {

        [Header("Hitscan Settings")]
        [SerializeField] private int damage = 10;
        [SerializeField] private float range = 100f;
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private Transform firePoint;
        [Tooltip("Amount general amount of ammo. We multiply this with magazine capasity")]
        [SerializeField] private int amountOfMagazines = 5;
        [SerializeField] private int magazineCapacity = 30;

        public int currentMaxAmmo;
        public int currentAmmo;

        Camera mainCamera;

        private void Awake()
        {
            if (firePoint == null) firePoint = transform;
            mainCamera = Camera.main;
            currentMaxAmmo = amountOfMagazines * magazineCapacity;
            currentAmmo = magazineCapacity;
        }

        public override void Fire(bool isPressed = true)
        {
            // Perform raycast from firePoint forward
            Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
            {
                var damagable = hit.collider.GetComponent<AniDrag.Core.IDamagable>();
                damagable?.TakeDamage(damage);
            }
        }
        public override void Aim(bool isPressed = true)
        {
            // Implement aiming logic (e.g., zoom in, change crosshair, etc.)
            Debug.Log("Aiming...");
        }
        public override void Reload(bool isPressed = true)
        {
            int ammoNeeded = magazineCapacity - currentAmmo;
            if (currentMaxAmmo >= ammoNeeded)
            {
                currentMaxAmmo -= ammoNeeded;
                currentAmmo = ammoNeeded;
            }
            else if(currentMaxAmmo < ammoNeeded && currentMaxAmmo > 0)
            {
                currentAmmo += currentMaxAmmo;
                currentMaxAmmo = 0;
            }
            else
            {
                Debug.Log("No ammo left to reload!");
            }
        }
    }
}
