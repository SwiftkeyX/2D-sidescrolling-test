using System;
using System.Collections.Generic;
using SideScroller.Interactions;
using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Storage
{
    /// A chest placed in the world. it was inventory for keeping item.
    /// interact with it to open/close it.
    [RequireComponent(typeof(Collider2D))]
    public class StorageChest : MonoBehaviour, IInteractable
    {
        [SerializeField] private int _size = 30;

        // pre-authored items
        [SerializeField] private List<ItemAmount> _startingItems = new();
        private Inventory _contents;

        public static event Action<StorageChest> Interacted;

        public Inventory Contents => _contents ??= PreAuthorItem();
        public void Interact() => Interacted?.Invoke(this);

        // ==================== private ====================
        private Inventory PreAuthorItem()
        {
            Inventory contents = new Inventory(_size);

            foreach (ItemAmount entry in _startingItems)
            {
                if (entry.Item == null) continue;

                for (int i = 0; i < entry.Count; i++)
                {
                    if (contents.TryAdd(entry.Item)) continue;

                    Debug.LogWarning($"{name}: starting items don't fit in {_size} cells, the rest are left out.", this);
                    return contents;
                }
            }

            return contents;
        }
    }
}
