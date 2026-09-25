using UnityEngine;

namespace SideScroller.Equipments
{
    /// <summary>
    /// The behaviour of an equipped tool. Lives on its own prefab, referenced by EquipmentSO.
    /// EquipmentSlot spawns it as a child of the player when equipped, and destroys it when swapped out.
    /// </summary>
    public abstract class Equipment : MonoBehaviour
    {
        public abstract void Activate(Vector2 direction);
    }
}
