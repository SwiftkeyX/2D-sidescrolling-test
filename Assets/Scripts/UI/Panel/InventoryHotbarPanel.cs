using SideScroller.Characters;
using SideScroller.Inventories;

namespace SideScroller.UI
{
    /// display player's hotbar as a panel. It was displayed at the bottom right of the screen.
    internal class InventoryHotbarPanel : InventoryPanel
    {
        protected override string GridName => "HotbarGrid";
        protected override Inventory PickInventory(Player player) => player.Hotbar;
    }
}
