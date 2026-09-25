using SideScroller.Farming;
using SideScroller.Inventories;
using SideScroller.Items;
using UnityEngine;

namespace SideScroller.Characters
{
    /// part of the Player: the backpack & hotbar, and what the player does with an item in a slot.
    /// 1) drop: the whole stack goes on the floor in front of the player
    /// 2) use: equip a tool / plant a seed / place a placeable
    public partial class Player : IItemCollector
    {
        private const float DropSpread = 0.3f;              // gap between items when a stack is dropped

        // created on first use: UI panels may ask for them before the Player's Awake
        private Inventory _backpack;
        private Inventory _hotbar;

        // ======================================== getter ========================================
        public Inventory Backpack => _backpack ??= new Inventory(_backpackSize);
        public Inventory Hotbar => _hotbar ??= new Inventory(_hotbarSize);

        // ======================================== interface ========================================
        // walking into an item lying in the world, collect it into the backpack
        public bool TryCollect(IInventoryable item) => Backpack.TryAdd(item);


        // ======================================== use ========================================
        // use the item in the slot. what "use" means depends on what kind of item it is (its type, not its category):
        // EquipmentSO => equip it. it stays in the slot, left click uses it
        // SeedSO      => plant it where the pointer is
        // PlaceableSO => put it down where the pointer is, e.g. storage chest
        // anything else, e.g. Lumber, Golden Veggie => nothing, it stays in the slot
        public void UseItem(Inventory from, int slot)
        {
            switch (from.Get(slot))
            {
                case EquipmentSO tool:
                    Equip(tool);
                    break;

                case SeedSO seed:
                    TryPlant(seed, from, slot, GetPointerWorldPosition());
                    break;

                case PlaceableSO placeable:
                    TryPlace(placeable, from, slot, GetPointerWorldPosition());
                    break;
            }
        }

        // plant the seed on the ground near the pointer.
        private void TryPlant(SeedSO seed, Inventory from, int slot, Vector2 worldPos)
        {
            // try plant it
            if (Plant.TryPlant(seed, worldPos) == null) return;

            // if success, take one seed off the stack
            from.Remove(slot);
        }

        // put the item down on the ground near the pointer
        // e.g. a storage chest
        private void TryPlace(PlaceableSO placeable, Inventory from, int slot, Vector2 worldPos)
        {
            // try place
            if (placeable.TryPlace(worldPos) == null) return;

            // if success, the placed item leaves the slot
            from.Remove(slot);
        }

        
        // ======================================== drop ========================================
        // take the whole stack out of the slot and drop it on the floor in front of the player
        public void DropItem(Inventory from, int slot)
        {
            ItemStack stack = from.GetStack(slot);
            if (stack == null) return;

            Vector2 front = (Vector2)transform.position + new Vector2(FacingDirection * _dropDistance, 0f);

            // drop item = spawn item into the world
            int dropped = 0;
            for (int i = 0; i < stack.Count; i++)
            {
                if (ItemPickup.Spawn(stack.Item, front + new Vector2(i * DropSpread, 0f)) == null) break;
                dropped++;
            }

            // remove dropped item from inventory
            if (dropped > 0) from.Remove(slot, dropped);
        }
    }
}
