using System;
using SideScroller.Characters;
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

        private Inventory _contents;
        public Inventory Contents => _contents ??= new Inventory(_size);

        public static event Action<StorageChest> Interacted;

        public void Interact(Player player) => Interacted?.Invoke(this);
    }
}
