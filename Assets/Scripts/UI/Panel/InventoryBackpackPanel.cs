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

        protected override string GridName => "BackpackGrid";
        protected override Inventory PickInventory(Player player) => player.Backpack;

        // =================================== public ===================================
        // open/close the window. dragging to re-arrange is only allowed while it is open (IsShown)
        public void Toggle() => SetShown(!IsShown);

        // =================================== Life cycle ===================================
        protected override void OnInventoryMounted(VisualElement panel)
        {
            SetShown(_startShown);
        }

        void Update()
        {
            if (PlayerInputSystem.InventoryPressedThisFrame) Toggle();
        }
    }
}
