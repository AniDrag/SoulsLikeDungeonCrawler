using UnityEngine;

namespace AniDrag.Core
{
    /// <summary>
    /// Items that can be equipped (weapons, armor, etc.).
    /// </summary>
    public interface IEquippable
    {
        EquipmentType EquipmentType { get; }
        GameObject WorldPrefab { get; }
        Stats EquipmentStats { get; }   
    }
}