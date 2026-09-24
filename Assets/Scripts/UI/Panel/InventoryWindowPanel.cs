using SideScroller.Characters;
using SideScroller.Input;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// display player's backpack as a panel.
    /// functionality on top of mother: 
    /// 1) open/close with O
    /// 2) drag to re-arrange
    /// 3) drag out to drop.
    internal class InventoryWindowPanel : InventoryPanelController
    {
        private const string GhostClass = "inventory__ghost";

        [SerializeField] private bool _startShown;

        private bool _shown;
        private VisualElement _ghost;
        private InventoryDragging _dragging;

        protected override string GridName => "BackpackGrid";
        protected override Inventory PickInventory(Player player) => player.Backpack;

        // =================================== public ===================================
        // open/close the window
        public void Toggle()
        {
            _shown = !_shown;
            SetShown(_shown);

            // closing the window mid-drag put the item back
            if (!_shown) _dragging?.Cancel();
        }

        // =================================== Life cycle ===================================
        protected override void OnInventoryMounted(VisualElement panel)
        {
            _shown = _startShown;
            SetShown(_shown);

            _ghost = BuildGhost();
            _dragging = new InventoryDragging(Inventory, Player, panel, Cells, _ghost);
        }

        void Update()
        {
            if (PlayerInputSystem.InventoryPressedThisFrame) Toggle();

            if (_shown) _dragging?.Tick();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _dragging?.Cancel();
            _ghost?.RemoveFromHierarchy();
        }

        // =================================== private ===================================
        // context: ghost = the icon that copy style from the item sprite. and can move follow player's pointer. 
        // it was to make a item sprite that can be dragged around.
        // this function is to init the ghost into the panel.
        private VisualElement BuildGhost()
        {
            VisualElement ghost = new VisualElement { pickingMode = PickingMode.Ignore };
            ghost.AddToClassList(GhostClass);
            ghost.style.display = DisplayStyle.None;

            // FIXLATER: This is kinda messy. It happen because the .uss isn't organize.
            // the ghost lives outside this panel, so it doesn't inherit the panel's .uss. hand it the same sheets
            for (int i = 0; i < Panel.styleSheets.count; i++)
            {
                ghost.styleSheets.Add(Panel.styleSheets[i]);
            }

            MainPanel.Add(ghost);
            return ghost;
        }
    }
}
