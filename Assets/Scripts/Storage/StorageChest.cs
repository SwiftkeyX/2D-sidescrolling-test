using System.Collections.Generic;
using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Storage
{
    // FIXME: Let implement interface Iinteractable, and give it to StorageChest and The seed that was place on the floor. 
    /// A chest placed in the world. it keeps its own inventory, bigger than the backpack.
    /// the player opens it with E when standing close, InventoryStoragePanel shows what is inside.
    public class StorageChest : MonoBehaviour
    {
        [SerializeField] private int _size = 30;

        // every chest in the scene, so the player can find the closest one without physics
        private static readonly List<StorageChest> All = new();

        private Inventory _contents;
        public Inventory Contents => _contents ??= new Inventory(_size);

        // the closest chest within "reach" of "position". null = none close enough
        public static StorageChest FindNearest(Vector2 position, float reach)
        {
            StorageChest nearest = null;
            float best = reach;

            foreach (StorageChest chest in All)
            {
                float distance = Vector2.Distance(position, chest.transform.position);
                if (distance > best) continue;

                best = distance;
                nearest = chest;
            }

            return nearest;
        }

        void OnEnable() => All.Add(this);
        void OnDisable() => All.Remove(this);
    }
}
