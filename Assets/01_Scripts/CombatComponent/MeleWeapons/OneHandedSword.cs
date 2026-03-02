using AniDrag.WeaponPack;
using UnityEngine;

// In the weapon prefab (actual sword script)
public class OneHandedSword : WeaponCore, IComboWeapon
{
    [Header("Combo Settings")]
    public float comboWindowDuration = 0.5f;   // time after hit to input next attack
    public float lightRecoveryTime = 0.2f;      // recovery after combo ends (light)
    public float heavyRecoveryTime = 0.8f;      // recovery after combo ends (heavy)
    public int maxComboSteps = 2;                // 2 attacks: vertical -> horizontal

    private Animator anim;
    private int currentComboStep = 0;
    private float comboWindowEndTime = 0f;
    private float recoveryEndTime = 0f;
    private bool isAttacking = false;
    private bool comboAvailable = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public override void Attack()
    {
        // If still in recovery, ignore input
        if (Time.time < recoveryEndTime) return;

        // If we are already attacking but in the combo window, treat as combo continuation
        if (isAttacking && comboAvailable)
        {
            ContinueCombo();
        }
        // Otherwise start a new combo
        else if (!isAttacking)
        {
            StartNewCombo();
        }
    }

    private void StartNewCombo()
    {
        isAttacking = true;
        currentComboStep = 0;
        PlayAttackAnimation(currentComboStep);
    }

    public void ContinueCombo()
    {
        if (!comboAvailable) return; // no longer in window

        // Move to next combo step, loop if desired
        currentComboStep++;
        if (currentComboStep >= maxComboSteps)
            currentComboStep = 0; // loop back to first attack

        PlayAttackAnimation(currentComboStep);
    }

    private void PlayAttackAnimation(int step)
    {
        // Use animation triggers: "Attack1", "Attack2", etc.
        anim.SetTrigger("Attack" + (step + 1));
        // The animation will call events at the right moments
    }

    // Called by animation event at the moment the weapon should hit
    public void OnHitFrame()
    {
        // Perform hit detection, damage, etc.
        Debug.Log("Hit on combo step " + currentComboStep);
    }

    // Called by animation event when the combo window opens (after hit)
    public void OnComboWindowOpen()
    {
        comboAvailable = true;
        comboWindowEndTime = Time.time + comboWindowDuration;
    }

    // Called by animation event when the combo window closes (e.g., at end of animation)
    public void OnComboWindowClose()
    {
        comboAvailable = false;
    }

    // Called by animation event when the entire attack animation finishes
    public void OnAttackFinished()
    {
        isAttacking = false;
        comboAvailable = false;

        // Determine recovery time based on weapon weight
        float recovery = (currentComboStep == maxComboSteps - 1) ? heavyRecoveryTime : lightRecoveryTime;
        recoveryEndTime = Time.time + recovery;

        // Optionally trigger a recovery animation
    }

    // IComboWeapon implementation
    public bool IsInComboWindow => comboAvailable && Time.time < comboWindowEndTime;
}
