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
    internal class InventoryBackpackPanel : InventoryPanel
    {
        [SerializeField] private bool _startShown;

        private bool _shown;

        protected override string GridName => "BackpackGrid";
        protected override Inventory PickInventory(Player player) => player.Backpack;

        // =================================== public ===================================
        // is the window open? dragging is only allowed while it is
        public bool IsShown => _shown;

        // open/close the window
        public void Toggle()
        {
            _shown = !_shown;
            SetShown(_shown);
        }

        // =================================== Life cycle ===================================
        protected override void OnInventoryMounted(VisualElement panel)
        {
            _shown = _startShown;
            SetShown(_shown);
        }

        void Update()
        {
            if (PlayerInputSystem.InventoryPressedThisFrame) Toggle();
        }
    }
}
