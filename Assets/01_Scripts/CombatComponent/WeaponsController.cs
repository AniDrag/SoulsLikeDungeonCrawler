using AniDrag.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
namespace AniDrag.WeaponPack
{
   
    [RequireComponent(typeof(PlayerInput))]
    public class WeaponsController : MonoBehaviour
    {
        [Header("References")]
        private WeaponCore currentWeapon;
        [SerializeField] private Transform HandL;
        [SerializeField] private Transform HandR;
        [SerializeField] private Camera camera;// Player camera // zoom in thats it
        [SerializeField] private PlayerInput inputs;


        [Header("Input action names [MELE]")]
        [SerializeField] private string attack = "Attack";
        [SerializeField] private string altAttack = "AltAttack";
        [SerializeField] private string block = "Block";

        [Header("Input action names [RANGED]")]
        [SerializeField] private string fire = "Fire";
        [SerializeField] private string altFire = "AltFire";
        [SerializeField] private string reload = "Reload";
        [SerializeField] private string aim = "Aim";

        [Header("Input action names [GENERAL COMBAT]")]
        [SerializeField] private string holster = "Holster";

        [Header("Debug variables")]
        [SerializeField] private GameObject Debug_StarterWeapon;

        private bool holstered = false;
        private void Awake()
        {
            camera = Camera.main;
            if(inputs == null)  
                inputs = GetComponent<PlayerInput>();
        }
        private void Update()
        {
            if(currentWeapon == null) return;
            
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

            //currentWeapon.Equip();
                Debug.Log($"Weapon equipped!,{currentWeapon.inputType.ToString()}");
        }
        public void HolsterWeapon()
        {
            holstered = !holstered;
            // holster animation is played.
            //set bool holstered to true in the animator
            currentWeapon.Unequip();
             Destroy(currentWeapon.gameObject);
             Debug.Log("Weapon holstered!");
        }

       

        void MeleInputMap()
        {
            if (inputs.actions[holster].triggered )
            {
                //Debug.Log("Equipping/holstering weapon.");
                if (holstered) 
                    currentWeapon.Equip(); // unholster if attack is pressed while holstered
                else
                    currentWeapon.Unequip(); // holster if attack is pressed while unholstered
            }

            if (holstered) return;

            if (inputs.actions[attack].triggered)
            {
                currentWeapon.Attack();
                
            }
            else if (inputs.actions[altAttack].triggered)
            {
                currentWeapon.AltAttack();
            }

            if (inputs.actions[block].IsPressed())
             {
                currentWeapon.Block();
             }
             else if (inputs.actions[block].WasReleasedThisFrame())
             {
                 currentWeapon.Block(false);
            }
            // Block action
        }
        void RangedInputMap()
        {
            if (inputs.actions[holster].triggered)
            {
                //Debug.Log("Equipping/holstering weapon.");
                if (holstered)
                {
                    currentWeapon.Equip(); // unholster if attack is pressed while holstered
                    holstered = false;
                }
                else
                {
                    currentWeapon.Unequip(); // holster if attack is pressed while unholstered
                    holstered = true;
                }
            }

            if (holstered) return;

            if (inputs.actions[fire].triggered)
            {
                currentWeapon.Fire();

            }
            else if (inputs.actions[altFire].triggered)
            {
                currentWeapon.AltFire();
            }

            if (inputs.actions[reload].triggered)
            {
                currentWeapon.Reload();

            }
            if (inputs.actions[aim].IsPressed())
            {
                currentWeapon.Aim(true);
            }
            else if (inputs.actions[aim].WasReleasedThisFrame())
            {
                currentWeapon.Aim(false);
            }
        }


        // Debuhg

        [Button]
        void Debug_EquipWeapon()
        {
            Invoke(nameof(Debug_EquipW), 1f);
        }
        void Debug_EquipW()
        {
            Equip(Debug_StarterWeapon);
        }
    }



}
