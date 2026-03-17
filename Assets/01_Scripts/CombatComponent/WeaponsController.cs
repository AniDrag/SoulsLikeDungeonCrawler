using AniDrag.Core;
using UnityEngine;

namespace AniDrag.WeaponPack
{
    public enum WeaponInputType
    {
        Melee,
        Ranged
    }

    public class WeaponsController : MonoBehaviour
    {
        [Header("Weapon Attach Points")]
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _leftHand;

        [Header("Debug (Optional)")]
        [SerializeField] private GameObject _debugStarterWeapon;

        private IEquipmentUser _equipmentUser;
        private IWeapon _currentWeapon;
        private GameObject _currentWeaponObject;
        private WeaponInputType _currentWeaponType;
        private bool _holstered = false;

        private void Awake()
        {
            _equipmentUser = GetComponent<IEquipmentUser>();
            if (_equipmentUser == null)
                Debug.LogError("WeaponsController requires an IEquipmentUser component on the same GameObject.");
        }

        private void OnEnable()
        {
            if (_equipmentUser != null)
                _equipmentUser.OnEquipmentChanged += HandleEquipmentChanged;
        }

        private void OnDisable()
        {
            if (_equipmentUser != null)
                _equipmentUser.OnEquipmentChanged -= HandleEquipmentChanged;
        }

        private void Start()
        {
            
            if (_debugStarterWeapon != null)
            {
                EquipWeaponPrefab(_debugStarterWeapon);
            }
            else
            {
                var equipped = _equipmentUser?.GetEquipped(EquipmentType.MainWeapon);
                if (equipped != null)
                    EquipFromIEquippable(equipped);
            }
        }

        private void Update()
        {
            if (Services.Input.HolsterPressed)
                ToggleHolster();

            if (_currentWeapon == null || _holstered) return;

            switch (_currentWeaponType)
            {
                case WeaponInputType.Melee:
                    // One‑shot attacks
                    if (Services.Input.AttackPressed)
                        _currentWeapon.Attack(true);
                    if (Services.Input.AltAttackPressed)
                        _currentWeapon.AltAttack(true);
                    // Block is hold
                    _currentWeapon.Block(Services.Input.BlockHeld);
                    break;

                case WeaponInputType.Ranged:
                    // One‑shot fire
                    if (Services.Input.FirePressed)
                        _currentWeapon.Attack(true);
                    if (Services.Input.AltFirePressed)
                        _currentWeapon.AltAttack(true);
                    // Aim is hold
                    _currentWeapon.Aim(Services.Input.AimHeld);
                    // Reload one‑shot
                    if (Services.Input.ReloadPressed)
                        _currentWeapon.Reload(true);
                    break;
            }
        }

        private void ToggleHolster()
        {
            if (_currentWeapon == null) return;
            _holstered = !_holstered;
            if (_holstered)
                _currentWeapon.Unequip();
            else
                _currentWeapon.Equip();
        }

        private void HandleEquipmentChanged(IEquipmentUser user)
        {
            var equipped = user.GetEquipped(EquipmentType.MainWeapon);
            if (equipped != null)
                EquipFromIEquippable(equipped);
            else
                UnequipCurrent();
        }

        private void EquipFromIEquippable(IEquippable equippable)
        {
            if (equippable.WorldPrefab == null)
            {
                Debug.LogError("Equippable item has no world prefab.");
                return;
            }
            EquipWeaponPrefab(equippable.WorldPrefab);
        }

        private void EquipWeaponPrefab(GameObject weaponPrefab)
        {
            if (_currentWeaponObject != null)
                Destroy(_currentWeaponObject);

            Transform attachPoint = _rightHand; // or determine by weapon type
            _currentWeaponObject = Instantiate(weaponPrefab, attachPoint.position, attachPoint.rotation, attachPoint);
            _currentWeapon = _currentWeaponObject.GetComponent<IWeapon>();

            if (_currentWeapon == null)
            {
                Debug.LogError($"Weapon prefab {weaponPrefab.name} does not have a component implementing IWeapon.");
                Destroy(_currentWeaponObject);
                return;
            }

            // Determine weapon input type – you could read this from a property on IWeapon
            // For now, assume it's based on a component or a tag; here we use a simple convention:
            // If the weapon has a RangedWeapon component, treat as ranged; otherwise melee.
            _currentWeaponType = _currentWeaponObject.GetComponent<RangedWeapon>() != null 
                ? WeaponInputType.Ranged 
                : WeaponInputType.Melee;

            _currentWeapon.Owner = gameObject;
            _currentWeapon.Equip();
            _holstered = false;
        }

        private void UnequipCurrent()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.Unequip();
                Destroy(_currentWeaponObject);
                _currentWeapon = null;
                _currentWeaponObject = null;
            }
        }
    }
}