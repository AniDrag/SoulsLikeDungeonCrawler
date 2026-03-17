using System.Collections.Generic;
using AniDrag.Core;
using UnityEngine;

namespace AniDrag.WeaponPack
{
    public class MeleeWeapon : WeaponCore
    {
        [Header("Animation")]
        [SerializeField] private string attackTrigger = "Attack";
        [SerializeField] private string comboInteger = "ComboStep";
        [SerializeField] private string blockBool = "Blocking";
        [SerializeField] private string holsterBool = "Holstered";

        [Header("Combo Settings")]
        [SerializeField] private int maxComboSteps = 3;
        [SerializeField] private float comboResetTime = 0.5f;
        [SerializeField] private float lightRecoveryTime = 0.2f;
        [SerializeField] private float heavyRecoveryTime = 0.8f;

        [Header("Damage")]
        [SerializeField] private int baseDamage = 10;
        [SerializeField] private LayerMask hitLayers;
        [SerializeField] private Transform[] hitPoints;
        [SerializeField] private float hitRadius = 0.5f;

        [Header("Stats Integration")]
        [SerializeField] private StatType damageStatMultiplier = StatType.STR; // e.g., STR increases damage

        private Animator anim;
        private int currentComboStep = 0;
        private float recoveryEndTime = 0f;
        private bool isAttacking = false;
        private bool comboAvailable = false;
        private bool bufferedAttackInput = false;
        private bool isSwingActive = false;
        private HashSet<GameObject> hitDuringSwing = new HashSet<GameObject>();
        private List<HitInfo> recentHits = new List<HitInfo>();

        private struct HitInfo
        {
            public Vector3 position;
            public float time;
            public int damage;
        }

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (recoveryEndTime > 0)
                recoveryEndTime -= Time.deltaTime;

            if (isSwingActive)
                PerformHitDetection();
        }
        public override bool CanAttack() => !isAttacking && Time.time >= recoveryEndTime && !bufferedAttackInput;
        public override bool IsAttacking() => isAttacking || isSwingActive;
        public override float GetAttackRange() => 2f; // approximate range based on hit points

        #region IWeapon Implementation

        public override void Attack(bool isPressed)
        {
            if (!isPressed) return; // only act on press down

            if (Time.time < recoveryEndTime || bufferedAttackInput) return;
            bufferedAttackInput = true;

            if (isAttacking && comboAvailable)
                ContinueCombo();
            else if (!isAttacking)
                StartNewCombo();
        }

        public override void AltAttack(bool isPressed)
        {
            // For now, treat as same as attack (or implement heavy attack later)
            Attack(isPressed);
        }

        public override void Block(bool isPressed)
        {
            // Your blocking logic (set animator bool, etc.)
            anim.SetBool("Blocking", isPressed);
        }

        public override void Aim(bool isPressed)
        {
            // Not used for melee; leave empty or handle as needed
        }

        public override void Reload(bool isPressed)
        {
            // Not used for melee
        }

        public override void Equip()
        {
            base.Equip();
            anim.SetBool("Holstered", false);
        }

        public override void Unequip()
        {
            base.Unequip();
            anim.SetBool("Holstered", true);
        }

        #endregion

        #region Animation Events

        private void StartNewCombo()
        {
            isAttacking = true;
            comboAvailable = true;
            currentComboStep = 0;
            anim.SetInteger("ComboStep", currentComboStep);
            anim.SetBool("CanAttack", true);
            anim.SetTrigger("Attack");
        }

        private void ContinueCombo()
        {
            if (!comboAvailable) return;
            currentComboStep++;
            if (currentComboStep >= maxComboSteps)
                currentComboStep = 0;
            anim.SetInteger("ComboStep", currentComboStep);
            anim.SetBool("CanAttack", true);
            anim.SetTrigger("Attack");
            comboAvailable = false;
        }

        // Animation events (keep your existing ones)
        public void AnimEv_OnSwingStart()
        {
            anim.SetBool("CanAttack", true);
            comboAvailable = true;
        }

        public void AnimEv_OnSwingEnd()
        {
            anim.SetBool("CanAttack", false);
            comboAvailable = false;
            isAttacking = false;
            bufferedAttackInput = false;
            float recovery = (currentComboStep == maxComboSteps - 1) ? heavyRecoveryTime : lightRecoveryTime;
            recoveryEndTime = Time.time + recovery;
        }

        public void AnimEv_OnHitStart()
        {
            isSwingActive = true;
            hitDuringSwing.Clear();
        }

        public void AnimEv_OnHitEnd()
        {
            isSwingActive = false;
            bufferedAttackInput = false;
        }

        #endregion

        private void PerformHitDetection()
        {
            if (hitPoints == null || hitPoints.Length == 0) return;

            int finalDamage = CalculateDamage();

            foreach (var point in hitPoints)
            {
                if (point == null) continue;

                Collider[] hits = Physics.OverlapSphere(point.position, hitRadius, hitLayers);
                foreach (var hit in hits)
                {
                    if (hitDuringSwing.Contains(hit.gameObject)) continue;

                    var damagable = hit.GetComponent<IDamagable>();
                    if (damagable != null)
                    {
                        damagable.TakeDamage(finalDamage, Owner);
                        hitDuringSwing.Add(hit.gameObject);

                        recentHits.Add(new HitInfo
                        {
                            position = hit.ClosestPoint(point.position),
                            time = Time.time,
                            damage = finalDamage
                        });
                    }
                }
            }
        }

        private int CalculateDamage()
        {
            int damage = baseDamage;

            // If owner has stats, multiply accordingly
            if (Owner != null)
            {
                var statsProvider = Owner.GetComponent<IStatsProvider>();
                if (statsProvider != null)
                {
                    Stats totalStats = statsProvider.GetTotalStats();
                    switch (damageStatMultiplier)
                    {
                        case StatType.STR:
                            damage += totalStats.STR * 2; // example scaling
                            break;
                        case StatType.DEX:
                            damage += totalStats.DEX * 2;
                            break;
                        // etc.
                    }
                }
            }

            return damage;
        }

        private void OnDrawGizmosSelected()
        {
            if (hitPoints == null) return;
            Gizmos.color = isSwingActive ? Color.red : Color.yellow;
            foreach (var point in hitPoints)
            {
                if (point != null)
                    Gizmos.DrawWireSphere(point.position, hitRadius);
            }
        }
    }
}