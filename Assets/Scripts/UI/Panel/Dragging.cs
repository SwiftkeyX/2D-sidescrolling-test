using System;
using SideScroller.Input;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    // Item in Inventory can be dragged. The drag it to achive those function:
    // 1) Picking items up with the pointer
    // 2) dragging item around to re-arrange them
    // 3) releasing it outside e.g. drop it, use it depending on the mode
    internal class Dragging
    {
        private readonly bool _canRearrange;
        private readonly Action<Inventory, int> _releasedOutside;

        // every panel the item can move between
        private readonly InventoryPanel[] _panels;
        // which panel slot is currently being hold
        private PanelSlot _held;

        // the icon that follows the pointer
        private readonly VisualElement _ghost;


        // ============================= getter =============================
        public bool IsHolding => _held != null;

        public Dragging(InventoryPanel[] panels, VisualElement ghost, bool canRearrange, Action<Inventory, int> releasedOutside)
        {
            _panels = panels;
            _ghost = ghost;
            _canRearrange = canRearrange;
            _releasedOutside = releasedOutside;
        }

        // =================================== life cycle ===================================
        // pickup, drag, drop item
        public void Tick()
        {
            Vector2 panelPos = Picker.ToPanel(_ghost.panel, PlayerInputSystem.PointerScreenPosition);

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

            // if not pointing at a slot with an item, nothing to grab
            PanelSlot slot = FindSlot(panelPos);
            if (slot == null || slot.Item == null) return;

            Grab(slot);
        }

        // grab the item using PanelSlot data
        private void Grab(PanelSlot slot)
        {
            _held = slot;

            // dim the cell where item is from. indicate that this item slot was currenly held.
            slot.Panel.ShowHeld(slot.Index, true);

            // when item is held, other inventory panel turn green to indicate it can be interacted with.
            foreach (InventoryPanel panel in _panels)
            {
                if (panel != slot.Panel) panel.ShowDroppable(true);
            }

            // ghost copy the item icon. 
            _ghost.style.backgroundImage = new StyleBackground(slot.Item.Icon);
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
        // 1) a cell, in any panel => move it there, swap if the cell already have item (if re-arranging is allowed)
        // 2) outside panel => hand resposibility to the owner 
        // the function could either be to "drop it on the floor" or to "use it"
        private void Drop(Vector2 panelPos)
        {
            PanelSlot target = FindSlot(panelPos);

            // released on a cell = put the item there
            if (target != null && _canRearrange)
            {
                PlaceAndSwap(target);
                Release();
                return;
            }

            // released outside every panel = let the owner decide
            // the owner = the one who set function to _releaseOutside
            if (target == null && IsPointerOutsidePanels(panelPos))
            {
                _releasedOutside?.Invoke(_held.Panel.Inventory, _held.Index);
                Release();
                return;
            }

            // released on nothing = put item back to its original slot
            Cancel();
        }

        // place the held item on the target slot
        // if the target slot already have item, swap the slot.
        private void PlaceAndSwap(PanelSlot target)
        {
            Inventory.SwapBetweenInventory(_held.Panel.Inventory, _held.Index, target.Panel.Inventory, target.Index);
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
            if (IsHolding) _held.Panel.ShowHeld(_held.Index, false);

            foreach (InventoryPanel panel in _panels) panel.ShowDroppable(false);

            _ghost.style.display = DisplayStyle.None;
            _held = null;
        }

        // ============================================= pointer =============================================
        // which slot, in which panel, is under the pointer?
        private PanelSlot FindSlot(Vector2 panelPos)
        {
            foreach (InventoryPanel panel in _panels)
            {
                int index = panel.SlotAt(panelPos);
                if (index >= 0) return new PanelSlot(panel, index);
            }

            return null;
        }

        // is player's pointer outside every panel?
        private bool IsPointerOutsidePanels(Vector2 panelPos)
        {
            foreach (InventoryPanel panel in _panels)
            {
                if (panel.IsPointerInside(panelPos)) return false;
            }

            return true;
        }

        // ============================================= held information =============================================
        // context: we have more than 1 inventory and those inventory should be able to swap item. 
        // so we need a way those inventory talk to each other.
        // this class, "PanelSlot" dedicated itself to answer above question: where is this held item come from? 
        // To be more specific: 
        // 1) which invenntory panel this item belong to? 
        // 2) which index of the said panel? 
        private class PanelSlot
        {
            public InventoryPanel Panel { get; }
            public int Index { get; }
            public IInventoryable Item => Panel.Inventory.Get(Index);

            public PanelSlot(InventoryPanel panel, int index)
            {
                Panel = panel;
                Index = index;
            }
        }
    }
}
