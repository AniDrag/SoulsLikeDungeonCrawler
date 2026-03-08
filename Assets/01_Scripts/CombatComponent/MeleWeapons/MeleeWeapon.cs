using System.Collections.Generic;
using UnityEngine;
using AniDrag.Core;
using AniDrag.Utility;

namespace AniDrag.WeaponPack
{
    // Ok Input attack -> AttackInputPressed. until end of hit that is true then its set to false. if that is true attack rturns
    public class MeleeWeapon : WeaponCore
    {
        [Header("anim strings")]
        [SerializeField] private string attack = "Attack";
        [SerializeField] private string combo = "ComboStep";
        [SerializeField] private string altAttack = "AltAttack";
        [SerializeField] private string holster = "HolsterAction"; 
        [SerializeField] private string enableAttack = "CanAttack";

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

        //Buffer for input during recovery or combo windows, can be expanded for more complex input handling
        private bool bufferedAttackInput = false;
        //private bool bufferedAltAttackInput = false;
        private bool bufferedHolsterInput= false;

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
            // Count down recovery time
            if (recoveryEndTime > 0)
                recoveryEndTime -= Time.deltaTime;

            // Hit detection while swing active
            if (isSwingActive)
                PerformHitDetection();
        }

        #region WeaponCore Main Functions
        public override void Attack(bool isPressed = true)
        {
            if (Time.time < recoveryEndTime || bufferedAttackInput) return;
            bufferedAttackInput = true;
            if (isAttacking && comboAvailable)
                ContinueCombo();
            else if (!isAttacking)
                StartNewCombo();
            //Debug.Log($"Attack called. IsAttacking: {isAttacking}, ComboAvailable: {comboAvailable}, CurrentComboStep: {currentComboStep}");    
        }
        public override void Equip()
        {
            if (bufferedHolsterInput) return;
            base.Equip();
            anim.SetBool(holster, true);
        }
        public override void Unequip()
        {
            base.Unequip();
            anim.SetBool(enableAttack, false);
            anim.SetTrigger(holster);
        }

        private void StartNewCombo()
        {
            isAttacking = true;
            comboAvailable = true; // will be refined by events
            currentComboStep = 0;
            anim.SetInteger(combo, currentComboStep);
            anim.SetBool(enableAttack, true);   // allow transition
            anim.SetTrigger(attack);
        }

        private void ContinueCombo()
        {
            if (!comboAvailable) return;

            currentComboStep++;
            if (currentComboStep >= maxChainAttacks)
                currentComboStep = 0; // or loop to first

            anim.SetInteger(combo, currentComboStep);
            anim.SetBool(enableAttack, true);   // ensure it's true for the next attack
            anim.SetTrigger(attack);

            // Combo window is now closed until the next hit end
            comboAvailable = false;
            comboWindowEndTime = 0; // cancel timer if any
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
                        damagable.TakeDamage(baseDamage, owner);
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
        #endregion

        #region Animations Events
        // --- Animation Event Methods ---
        // Called by animation event when the swing begins
        public void AnimEv_OnSwingStart()
        {
            anim.SetBool(enableAttack, true);
            comboAvailable = true;
            //Debug.Log($"AnimEv_OnSwingStart: CanAttack = {anim.GetBool(enableAttack)}");
        }

        // Called by animation event when the swing ends
        public void AnimEv_OnSwingEnd()
        {
            anim.SetBool(enableAttack, false);
            comboAvailable = false;
            isAttacking = false;
            bufferedAttackInput = false;

            float recovery = (currentComboStep == maxChainAttacks - 1) ? heavyRecoveryTime : lightRecoveryTime;
            recoveryEndTime = Time.time + recovery;
            //Debug.Log($"AnimEv_OnSwingEnd: CanAttack = {anim.GetBool(enableAttack)}");
        }
        public void AnimEv_OnHolsterStart()
        {
           
        }

        // Called by animation event when the swing ends
        public void AnimEv_OnHolsterEnd()
        {
            
        }
        public void AnimEv_OnHitStart()
        {
            isSwingActive = true;
            hitDuringSwing.Clear(); // new swing, new enemies
        }

        // Called by animation event when the swing ends
        public void AnimEv_OnHitEnd()
        {
            isSwingActive = false;
            bufferedAttackInput = false;
            // Optionally trigger combo window, recovery, etc.
        }
        #endregion


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
        public void AnimEv_Hit()
        {
            isSwingActive = !isSwingActive;
            if (isSwingActive)
                hitDuringSwing.Clear();
            
        }
    }
}