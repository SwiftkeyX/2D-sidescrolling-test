using SideScroller.Input;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    // Picking items up with the pointer, dragging around, and putting them down again.
    internal class InventoryDragging
    {
        private const string HeldClass = "inventory__cell--held";

        private readonly Inventory _inventory;
        private readonly VisualElement _inventoryWindow;     // the inventory bound. drop outside it = drop the item
        private readonly VisualElement[] _cells;
        private readonly VisualElement _ghost;      // the icon that follows the pointer

        private int _held = -1;

        // ============================= getter =============================
        public bool IsHolding => _held >= 0;

        public InventoryDragging(Inventory inventory, VisualElement window, VisualElement[] cells, VisualElement ghost)
        {
            _inventory = inventory;
            _inventoryWindow = window;
            _cells = cells;
            _ghost = ghost;
        }

        // =================================== life cycle ===================================
        // pickup, drag, drop item
        public void Tick()
        {
            Vector2 panelPos = Picker.ToPanel(_inventoryWindow.panel, PlayerInputSystem.PointerScreenPosition);

            // if not holding anything, polling until something was holded
            if (!IsHolding)
            {
                TryPickUp(panelPos);
                return;
            }

            // the holding icon follow the pointer, create a visually object dragging
            Follow(panelPos);

            // if still holding, return
            bool stillHolding = PlayerInputSystem.IsPointerDown && !PlayerInputSystem.DragReleasedThisFrame;
            if (stillHolding) return;

            // if releasing, drop the item
            Drop(panelPos);
        }

        // ============================================= pick up =============================================
        // grab item using pointer
        private void TryPickUp(Vector2 panelPos)
        {
            if (!PlayerInputSystem.DragPressedThisFrame) return;

            int slot = TryPickItem(panelPos);
            if (slot >= 0) Grab(slot);
        }

        // get the item from the slot that was under the pointer
        private int TryPickItem(Vector2 panelPos)
        {
            int slot = Picker.At(_inventoryWindow.panel, panelPos, _cells);

            // if not picking up anything, return -1 
            if (slot < 0 || _inventory.Get(slot) == null) return -1;

            return slot;
        }

        // the item is holded by the player. 
        private void Grab(int slot)
        {
            _held = slot;

            // add heldclass to the cell. making it visually dim. indicate that this slot was currenly held.
            _cells[slot].AddToClassList(HeldClass);

            // ghost copy the item icon. ghost will follow the player's pointer to create visaully object dragging.
            _ghost.style.backgroundImage = new StyleBackground(_inventory.Get(slot).Icon);
            _ghost.style.display = DisplayStyle.Flex;
        }

        // make ghost follow player's pointer
        private void Follow(Vector2 panelPos)
        {
            // centre the ghost on the pointer
            _ghost.style.left = panelPos.x - _ghost.resolvedStyle.width / 2f;
            _ghost.style.top = panelPos.y - _ghost.resolvedStyle.height / 2f;
        }

        // ============================================= drop =============================================
        // an item can be dropped on 2 thing:
        // 1) a cell => move it there, swap if the cell already have item
        // 2) outside the inventory window => drop the item
        private void Drop(Vector2 panelPos)
        {
            int targetSlot = Picker.At(_inventoryWindow.panel, panelPos, _cells);

            // released on a cell = put the item there
            if (targetSlot >= 0)
            {
                PlaceAndSwap(_held, targetSlot);
                Release();
                return;
            }

            // released outside the window = drop the item
            if (IsPointerOutsideWindow(panelPos))
            {
                DropItem(_held);
                Release();
                return;
            }

            // released on nothing = put item back to its original slot
            Cancel();
        }

        // place the item on the target slot
        // if the target slot already have item, swap the slot.
        private void PlaceAndSwap(int holded, int targetSlot)
        {
            if (holded == targetSlot) return;

            _inventory.Swap(holded, targetSlot);
        }

        // FIXME: the item is gone for good. should spawn an ItemPickup into the world
        private void DropItem(int slot)
        {
            _inventory.Remove(slot);
        }

        // put back whatever is held
        // the inventory never changed while holding, so nothing to snap back
        public void Cancel()
        {
            Release();
        }

        // when stop holding, reset var
        private void Release()
        {
            if (IsHolding) _cells[_held].RemoveFromClassList(HeldClass);

            _ghost.style.display = DisplayStyle.None;
            _held = -1;
        }

        // ============================================= window bound =============================================
        // is player's pointer outside inventory boundary?
        private bool IsPointerOutsideWindow(Vector2 panelPos)
        {
            return !_inventoryWindow.worldBound.Contains(panelPos);
        }
    }
}
