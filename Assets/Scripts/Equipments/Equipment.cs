using UnityEngine;

namespace SideScroller.Equipments
{
    /// <summary>
    /// The behaviour of an equipped tool. Lives on its own prefab, referenced by EquipmentSO.
    /// e.g. wand shoot cube, axes cut tree
    /// </summary>
    public abstract class Equipment : MonoBehaviour
    {
        // direction = from the player toward the pointer (normalized)
        // pointer   = where the pointer is in the world
        public abstract void Activate(Vector2 direction, Vector2 pointer);
    }
}
