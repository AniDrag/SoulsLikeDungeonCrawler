using AniDrag.AI;
using AniDrag.WeaponPack;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack/Ranged")]
public class Attack_Ranged : AICore_AttackStrategy
{
    public float fireCooldown = 0.8f;
    public bool aimAtTarget = true;

    private float lastFireTime;

    public override void Initialize(AICore_Controller ai)
    {
        lastFireTime = -fireCooldown;
    }

    public override void OnEnterAttackRange(AICore_Controller ai, Transform target)
    {
        if (aimAtTarget)
        {
            // Start aiming (if weapon supports it)
            ai.weapon?.Aim(true);
        }
    }

    public override void OnExitAttackRange(AICore_Controller ai, Transform target)
    {
        if (aimAtTarget)
        {
            ai.weapon?.Aim(false);
        }
    }

    public override void UpdateAttack(AICore_Controller ai, Transform target)
    {
        // Continuously face the target while attacking
        if (aimAtTarget)
        {
            Vector3 direction = (target.position - ai.transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
                ai.transform.rotation = Quaternion.LookRotation(direction);
        }

        if (Time.time >= lastFireTime + fireCooldown)
        {
            ai.weapon?.Fire();
            lastFireTime = Time.time;
        }
    }
}