using SideScroller.Characters;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// the player's own 2 craft cells, shown under the backpack.
    /// functionality on top of mother:
    /// 1) open/close together with the backpack
    internal class BackpackCraftPanel : CraftPanel
    {
        [SerializeField] private InventoryBackpackPanel _backpackPanel;

        protected override string GridName => "CraftGrid";
        protected override Inventory PickInventory(Player player) => player.CraftGrid;

        // =================================== Life cycle ===================================
        protected override void OnCraftMounted(VisualElement panel)
        {
            if (_backpackPanel == null) _backpackPanel = FindFirstObjectByType<InventoryBackpackPanel>();

            SetShown(false);
        }

        void Update()
        {
            if (_backpackPanel == null) return;

            // if backpack is open or closed, this craft panel also toggle too. 
            if (IsShown != _backpackPanel.IsShown) SetShown(_backpackPanel.IsShown);
        }
    }
}
