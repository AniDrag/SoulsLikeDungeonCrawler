using UnityEngine;
using AniDrag.Core;

[CreateAssetMenu(fileName = "Sword", menuName = "AniDrag/Items/Equipment")]
public class EquipmentItem : Item
{
    public override bool Use(GameObject owner)
    {
        var user = owner.GetComponent<IEquipmentUser>();
        if (user != null)
        {
            user.Equip(this);
            return true;
        }
        return false;
    }
}

