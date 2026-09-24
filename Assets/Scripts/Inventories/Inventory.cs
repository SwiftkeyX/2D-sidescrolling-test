using System;

namespace SideScroller.Inventories
{
    // the player's items, one per slot. plain class, the owner (Player) holds it
    public class Inventory
    {
        private readonly IInventoryable[] _slots;
        public event Action<int, IInventoryable> SlotChanged;

        public int Size => _slots.Length;

        public Inventory(int size)
        {
            _slots = new IInventoryable[size];
        }

        public IInventoryable Get(int index) => IsValid(index) ? _slots[index] : null;

        // put the item in the very first empty slot. false when the inventory is full
        public bool TryAdd(IInventoryable item)
        {
            if (item == null) return false;

            int index = FirstEmptySlot();
            if (index < 0) return false;

            Set(index, item);
            return true;
        }

        // remove the item
        public void Remove(int index)
        {
            if (!IsValid(index)) return;

            Set(index, null);
        }

        // ==================== private ====================
        private int FirstEmptySlot()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null) return i;
            }

            return -1;
        }

        private void Set(int index, IInventoryable item)
        {
            _slots[index] = item;
            SlotChanged?.Invoke(index, item);
        }

        private bool IsValid(int index) => index >= 0 && index < _slots.Length;
    }
}
