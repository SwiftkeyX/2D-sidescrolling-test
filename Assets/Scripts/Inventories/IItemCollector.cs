using SideScroller.Items;

namespace SideScroller.Inventories
{
    // anything that can pick up an item lying in the world 
    // e.g. the player.
    public interface IItemCollector
    {
        bool TryCollect(IInventoryable item);
    }
}
