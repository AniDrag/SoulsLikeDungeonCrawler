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

        private void Awake()
        {
            if (firePoint == null) firePoint = transform;
        }

        public override void Attack()
        {
            // Perform raycast from firePoint forward
            Ray ray = new Ray(firePoint.position, firePoint.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
            {
                var damagable = hit.collider.GetComponent<AniDrag.Core.IDamagable>();
                damagable?.TakeDamage(damage);
            }
        }
    }
}
