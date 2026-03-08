using AniDrag.Core;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
namespace AniDrag.WeaponPack
{

    /// <summary>
    /// A projectile that can be fired by a weapon.
    /// - Deals damage on impact.
    /// - Tracks the owner (who shot it).
    /// - Self?destructs after impact or lifetime.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private int damage = 10;

        [Header("Lifetime")]
        [SerializeField] private float maxLifetime = 5f;          // Auto?destroy after this time
        [SerializeField] private bool destroyOnImpact = true;      // Destroy immediately upon hitting something
        [SerializeField] private float impactDestroyDelay = 0f;    // Optional delay before destruction

        [Header("Effects")]
        [SerializeField] private GameObject impactEffectPrefab;    // Optional visual effect on hit

        // Public property to set the owner (the GameObject that fired this projectile)
        public GameObject Owner { get; set; }

        private Rigidbody rb;
        private float spawnTime;
        public void Initialize(GameObject owner)
        {
            Owner = owner;
        }
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            spawnTime = Time.time;
        }

        private void Update()
        {
            // Auto?destroy after maxLifetime
            if (Time.time - spawnTime > maxLifetime)
                Destroy(gameObject);
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
            // Ignore the owner of the projectile
            if (Owner != null && other.gameObject == Owner)
                return;

            // Try to damage the object
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.TakeDamage(damage, Owner);
                // (Optional) If you want to give XP on kill, you could check here if the object died,
                // but that would require a more complex system (e.g., death event). For simplicity,
                // we leave XP to be handled elsewhere.
            }

            // Spawn impact effect
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destroy projectile (with optional delay)
            if (destroyOnImpact)
            {
                if (impactDestroyDelay > 0f)
                    Destroy(gameObject, impactDestroyDelay);
                else
                    Destroy(gameObject);
            }
        }

        // Optional: set initial velocity (called by weapon when spawning)
        public void SetVelocity(Vector3 velocity)
        {
            if (rb != null)
                rb.linearVelocity = velocity;
        }
    }
}