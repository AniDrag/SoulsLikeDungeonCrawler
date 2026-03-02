using UnityEngine;
namespace AniDrag.WeaponPack
{
    public enum WeaponInputType
    {
        Melee,
        Ranged,
    }
    public abstract class WeaponCore : MonoBehaviour
    {
        /*
        > concept: Pure functionality interface as a parent Base Weapon class. 
        > Give basic Logic calls we can modify then.
        > Helper functions that will help with universal integration.

        > use controll skeme type are just types.
            I. Mele weappon:
                - stwohanded,
                - 2 swords,
                - sword and shild,
                - charged weapons

            II. Ranged weapon:
                - chaged weapons
                - charge with aim
                - ranged raytracing
                - ranged physical
            This will be modified with values like charge time and stuff, that he controller will then see.


        > purpous of this? 
            Pure logic of how the weapon works.
            Integrated with the animator that will send transition triggers or helper details.
            Integration with Item aka Stat system. weapons that are affected with stats and whitch are not.
            Flexible enough to make an FPS military game or a RPG medival game.

        > Containing Conectores?
            - Using weapon functions aka virtual voids.
          */
        public WeaponInputType inputType;
        public abstract void Attack();          // called by controller on input
        public virtual void AltAttack() { }       // optional
        public virtual void Equip() { }
        public virtual void Unequip() { }

    }
}
