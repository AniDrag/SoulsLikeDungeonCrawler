
using System;

namespace AniDrag.Core
{
    /// <summary>
    /// Objects that can equip items (players, NPCs).
    /// </summary>
    public interface IEquipmentUser
    {
        void Equip(IEquippable item);
        void Unequip(EquipmentType slot);
        IEquippable GetEquipped(EquipmentType slot);
        
        // Event for when equipment changes
        event Action<IEquipmentUser> OnEquipmentChanged;
    }
}