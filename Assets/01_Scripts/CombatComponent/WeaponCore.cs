using AniDrag.Core;
using UnityEngine;

namespace AniDrag.WeaponPack
{
    public abstract class WeaponCore : MonoBehaviour, IWeapon
    {
        public IWeaponInputType inputType;
        public Animator weaponAnimator;

        private GameObject _owner;

        public GameObject Owner
        {
            get => _owner;
            set => _owner = value;
        }

        public virtual bool CanAttack(){return false;}

        public virtual bool IsAttacking() {return false;}

        public virtual float GetAttackRange(){return 0;}
        
        // Melee virtual methods
        public virtual void Attack(bool isPressed)
        {
            #if UNITY_EDITOR
            Debug.Log("[WEAPON - ATTACK ] Triggered / Pressed");
            #endif
        }

        public virtual void AltAttack(bool isPressed)
        {
#if UNITY_EDITOR
            Debug.Log("[WEAPON - Alt ATTACK ] Triggered / Pressed");
#endif
        }

        public virtual void Block(bool isPressed)
        {
#if UNITY_EDITOR
            Debug.Log("[WEAPON - BLOCK ] Triggered / Pressed");
#endif
        }
        public virtual void Aim(bool isPressed)
        {
#if UNITY_EDITOR
            Debug.Log("[WEAPON - AIM ] Triggered / Pressed");
#endif
        }

        // Ranged virtual methods
        public virtual void Reload(bool isPressed)
        {
#if UNITY_EDITOR
            Debug.Log("[WEAPON - RELOAD ] Triggered / Pressed");
#endif
        }
        // Could add Aim, Fire, etc. as separate methods if needed, but Attack covers both.

        public virtual void Equip()
        {
#if UNITY_EDITOR
            Debug.Log("Weapon equipped");
#endif
        }

        public virtual void Unequip()
        {
#if UNITY_EDITOR
            Debug.Log("Weapon unequipped");
#endif
            
        }
    }
}
