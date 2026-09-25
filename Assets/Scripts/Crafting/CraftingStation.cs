using System;
using SideScroller.Characters;
using SideScroller.Interactions;
using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Crafting
{
    /// A crafting station standing in the world. 
    /// it has its own 3 craft cells which is one more than the backpack's 2.
    [RequireComponent(typeof(Collider2D))]
    public class CraftingStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private int _size = 3;

        private Inventory _grid;

        public static event Action<CraftingStation> Interacted;

        public Inventory Grid => _grid ??= new Inventory(_size);
        public void Interact(Player player) => Interacted?.Invoke(this);
    }
}
