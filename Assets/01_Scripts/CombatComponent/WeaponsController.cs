using UnityEngine;
using UnityEngine.InputSystem;
namespace AniDrag.WeaponPack
{
    /* ignore this jsut an idea, ok Dowe?
    > concept. recive weapon gameobject. 
    > extract animator controller
    > extract weaponCore component type
    > use controll skeme type 
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

    > purpous of this? 
        Controll weapon actions universily.
        Integration with Equipment manager
        Integration with Item ID ssytem

    > Containing Conectores?
        - Using weapon
        - Mod weaponStats.

     */

    public class WeaponsController : MonoBehaviour
    {
        private WeaponCore currentWeapon;
        [SerializeField] private Transform HandL;
        [SerializeField] private Transform HandR;
        [SerializeField] private Camera camera;// Player camera // zoom in thats it

        // Inputs
        [SerializeField] private PlayerInput attack;
        [SerializeField] private PlayerInput altAttack;
        [SerializeField] private PlayerInput reload;
        [SerializeField] private PlayerInput aim;
        [SerializeField] private PlayerInput altFire;

        /// <summary>
        /// Equip a new weapon by instantiating its prefab as a child.
        /// </summary>
        /// <param name="weaponPrefab">The weapon prefab (must have a WeaponCore component).</param>
        public void Equip(GameObject weaponPrefab)
        {
            if (currentWeapon != null)
            {
                currentWeapon.Unequip();
                Destroy(currentWeapon.gameObject);
                Debug.Log("Weapon deleted!");
            }

            GameObject go = Instantiate(weaponPrefab, HandR);

            currentWeapon = go.GetComponent<WeaponCore>();

            if (currentWeapon == null)
            {
                Debug.LogError($"Weapon prefab '{weaponPrefab.name}' has no WeaponCore component on its root! Destroying instantiated object.");
                Destroy(go);
                return;
            }

            currentWeapon.Equip();
        }

        private void Update()
        {
            switch (currentWeapon.inputType)
            {
                case WeaponInputType.Melee:
                    MeleInputMap();
                    break;
                case WeaponInputType.Ranged:
                    RangedInputMap();
                    break;
            }
        }

        void MeleInputMap()
        {

        }
        void RangedInputMap()
        {

        }
    }


    
}
