namespace SideScroller.Inventories
{
    // ItemStack is what keep in the Inventory.cs
    // ItemStack is to group the "Item" and "Count" in the same class, so any item can stack itself inside inventory.cs 
    public class ItemStack
    {
        public IInventoryable Item { get; }
        public int Count { get; internal set; }

        public int Room => Item.MaxStack - Count;
        public bool IsFull => Room <= 0;

        public ItemStack(IInventoryable item, int count = 1)
        {
            Item = item;
            Count = count;
        }

        // allow merge, if the added item was:
        // 1) the same item to this stack
        // 2) stack still has room
        public bool CanMerge(IInventoryable item) => item == Item && !IsFull;
    }
}
