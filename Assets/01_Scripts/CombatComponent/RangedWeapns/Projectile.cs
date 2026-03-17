using AniDrag.Core;
using UnityEngine;
namespace AniDrag.WeaponPack
{
    /*
     Projectile has its own damage value and lifetime.
    It can be used for various types of ranged weapons (guns, bows, magic spells, etc.).
    it should know what layer to hit. well better to just ignore its owner object and hit everything else, that way it can be used for both player and enemy projectiles without needing separate layers.

    so variables:
        - Damage amount
        - Lifetime (time before auto?destroy)
        - boince count (optional, for projectiles that can bounce)
        - mass.
        - Destroy on impact (bool)
        - Impact effect (optional prefab to spawn on hit)  
        - owner (the GameObject that fired the projectile, used to ignore collisions and for XP attribution)
        - rigidbody reference (for physics-based movement, optional if you want to use transform-based movement instead) and then turn kinematic on impact if we want it to.
        - spawn time (to track lifetime)
        - explosionRadius (optional, for explosive projectiles that damage in an area on impact)
        - dawmage falloff (optional, for projectiles that deal less damage the farther they travel or from the explosion center)
     
     
     
     */

    /// <summary>
    /// A projectile that can be fired by a weapon.
    /// - Deals damage on impact.
    /// - Tracks the owner (who shot it).
    /// - Self?destructs after impact or lifetime.
    /// </summary>
      [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [Header("Damage")]
        public int damage = 10;

        [Header("Lifetime")]
        [SerializeField] private float maxLifetime = 5f;
        [SerializeField] private bool destroyOnImpact = true;
        [SerializeField] private float impactDestroyDelay = 0f;

        [Header("Effects")]
        [SerializeField] private GameObject impactEffectPrefab;

        [Header("Collision & Behaviour")]
        [SerializeField] private LayerMask hitLayers = -1;
        [SerializeField] private bool friendlyFire = false;
        [SerializeField] private bool stickToEnvironment = true;
        [SerializeField] private float stickDuration = 4f;

        public GameObject Owner { get; set; }
        public int TeamId { get; set; } = -1; // -1 = neutral

        private Rigidbody rb;
        private float spawnTime;
        private bool hasImpacted = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            spawnTime = Time.time;
        }

        private void Update()
        {
            if (!hasImpacted && Time.time - spawnTime > maxLifetime)
                Destroy(gameObject);
        }

        public void Initialize(GameObject owner, int teamId = -1)
        {
            Owner = owner;
            TeamId = teamId;
        }

        public void SetVelocity(Vector3 velocity)
        {
            if (rb != null)
                rb.linearVelocity = velocity;
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleImpact(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleImpact(collision.collider);
        }

        private void HandleImpact(Collider other)
        {
            if (hasImpacted) return;

            // Ignore owner
            if (Owner != null && other.gameObject == Owner)
                return;

            // Check layer mask
            if (!IsInLayerMask(other.gameObject.layer, hitLayers))
                return;

            // Team filtering
            if (!friendlyFire)
            {
                var teamIdentifier = other.GetComponent<ITeamIdentifier>();
                if (teamIdentifier != null && teamIdentifier.TeamId == TeamId && TeamId != -1)
                    return;
            }

            hasImpacted = true;

            var damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
                damagable.TakeDamage(damage, Owner);

            if (impactEffectPrefab != null)
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

            if (damagable == null && stickToEnvironment)
                StickToSurface(other.transform);
            else if (destroyOnImpact)
                Destroy(gameObject, impactDestroyDelay);
        }

        private void StickToSurface(Transform parent)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            transform.SetParent(parent);

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Destroy(gameObject, stickDuration);
        }

        private bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return (layermask.value & (1 << layer)) != 0;
        }
    }
}