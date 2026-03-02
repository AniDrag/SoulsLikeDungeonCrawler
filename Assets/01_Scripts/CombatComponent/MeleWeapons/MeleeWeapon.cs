using System.Collections.Generic;
using UnityEngine;
using AniDrag.Core;
using AniDrag.Utility;

namespace AniDrag.WeaponPack
{
    public class MeleeWeapon : WeaponCore
    {
        [Header("Combo Settings")]
        [SerializeField] private float comboWindowDuration = 0.5f;
        [SerializeField] private float lightRecoveryTime = 0.2f;
        [SerializeField] private float heavyRecoveryTime = 0.8f;
        [SerializeField] private int maxChainAttacks = 2;

        [Header("Damage")]
        [SerializeField] private int baseDamage = 10;
        [SerializeField] private LayerMask targetLayers;

        [Header("Hit Points")]
        [Tooltip("Multiple points along the blade where hit detection occurs.")]
        [SerializeField] private Transform[] hitPoints; // assign child transforms in prefab
        [SerializeField] private float hitRadius = 0.5f; // radius for each sphere
        private bool isSwingActive = false;
        private HashSet<GameObject> hitDuringSwing = new HashSet<GameObject>();

        [Header("Debug")]
        [Tooltip("How long hit locations are shown for.")]
        [SerializeField] private float hitGizmoDuration = 1f;
        [Tooltip("if we dont want any gizmos")]
        [SerializeField] private bool enableGizmos = false;
        [Tooltip("for editingf and stuff colliders will always be on")]
        [SerializeField] private bool constantShowHitColliders = true;

        private Animator anim;
        private int currentComboStep = 0;
        private float comboWindowEndTime = 0f;
        private float recoveryEndTime = 0f;
        private bool isAttacking = false;
        private bool comboAvailable = false;

        private struct HitInfo
        {
            public Vector3 position;
            public float time;
            public int damage;
        }
        private List<HitInfo> recentHits = new List<HitInfo>();

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }
        private void Update()
        {
            // If swing is active, check for hits every frame
            if (isSwingActive)
            {
                PerformHitDetection();
            }
        }

        public override void Attack()
        {
            if (Time.time < recoveryEndTime) return;

            if (isAttacking && comboAvailable)
                ContinueCombo();
            else if (!isAttacking)
                StartNewCombo();
        }

        private void StartNewCombo()
        {
            isAttacking = true;
            currentComboStep = 0;
            PlayAttackAnimation(currentComboStep);
        }

        private void ContinueCombo()
        {
            if (!comboAvailable) return;

            currentComboStep++;
            if (currentComboStep >= maxChainAttacks)
                currentComboStep = 0;

            PlayAttackAnimation(currentComboStep);
        }

        private void PlayAttackAnimation(int step)
        {
            anim.SetTrigger("Attack" + (step + 1));
        }

        // --- Animation Event Methods ---
        // Called by animation event when the swing begins
        public void OnSwingStart()
        {
            isSwingActive = true;
            hitDuringSwing.Clear(); // new swing, new enemies
        }

        // Called by animation event when the swing ends
        public void OnSwingEnd()
        {
            isSwingActive = false;
            // Optionally trigger combo window, recovery, etc.
        }

        private void PerformHitDetection()
        {
            if (hitPoints == null || hitPoints.Length == 0) return;

            foreach (var point in hitPoints)
            {
                if (point == null) continue;

                Collider[] hits = Physics.OverlapSphere(point.position, hitRadius, targetLayers);
                foreach (var hit in hits)
                {
                    // Skip if already hit this swing
                    if (hitDuringSwing.Contains(hit.gameObject))
                        continue;

                    var damagable = hit.GetComponent<IDamagable>();
                    if (damagable != null)
                    {
                        damagable.TakeDamage(baseDamage);
                        hitDuringSwing.Add(hit.gameObject);

                        // Record hit for debug
                        recentHits.Add(new HitInfo
                        {
                            position = hit.ClosestPoint(point.position),
                            time = Time.time,
                            damage = baseDamage
                        });
                    }
                }
            }
        }

        public void OnComboWindowOpen()
        {
            comboAvailable = true;
            comboWindowEndTime = Time.time + comboWindowDuration;
        }

        public void OnComboWindowClose()
        {
            comboAvailable = false;
        }

        public void OnAttackFinished()
        {
            isAttacking = false;
            comboAvailable = false;

            float recovery = (currentComboStep == maxChainAttacks - 1) ? heavyRecoveryTime : lightRecoveryTime;
            recoveryEndTime = Time.time + recovery;
        }

        private void OnDrawGizmos()
        {
            if (!enableGizmos) return;
            // Draw hit point spheres
            if (hitPoints != null && isSwingActive || constantShowHitColliders)
            {
                Gizmos.color = isSwingActive? Color.cadetBlue : Color.yellow;
                foreach (var point in hitPoints)
                {
                    if (point != null)
                        Gizmos.DrawWireSphere(point.position, hitRadius);
                }
            }

            // Draw recent hit positions
            if (recentHits == null) return;

            recentHits.RemoveAll(h => Time.time - h.time > hitGizmoDuration);

            Gizmos.color = Color.red;
            foreach (var hit in recentHits)
            {
                Gizmos.DrawSphere(hit.position, 0.1f);
            }
        }
        [Button("Test Hit")]
        void TestHit()
        {
            isSwingActive = !isSwingActive;
            if (isSwingActive)
                hitDuringSwing.Clear();
            
        }
    }
}