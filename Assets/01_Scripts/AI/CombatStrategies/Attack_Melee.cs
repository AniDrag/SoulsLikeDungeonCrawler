using AniDrag.AI;
using AniDrag.WeaponPack;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack/Melee")]
public class Attack_Melee : AICore_AttackStrategy
{
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    public override void Initialize(AICore_Controller ai)
    {
        lastAttackTime = -attackCooldown; // allow immediate first attack
    }

    public override void UpdateAttack(AICore_Controller ai, Transform target)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Call the weapon's Attack method (melee)
            ai.weapon?.Attack();
            lastAttackTime = Time.time;

            // Optional: face the target
            Vector3 direction = (target.position - ai.transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
                ai.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}