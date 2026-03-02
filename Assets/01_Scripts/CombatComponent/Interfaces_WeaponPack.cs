using UnityEngine;
namespace AniDrag.WeaponPack
{
    // Interface for weapons that support combos
    public interface IComboWeapon
    {
        void ContinueCombo(); 
        bool IsInComboWindow { get; }
    }
    public interface IChargeable
    {
        void StartCharge();
        void UpdateCharge(float chargeAmount);
        void ReleaseCharge();
    }
    public interface IAimable
    {
        void StartAim();
        void UpdateAim(Vector2 aimInput);
        void StopAim();
    }
}