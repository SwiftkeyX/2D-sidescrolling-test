using System;

namespace SideScroller.Inventories
{
    // the player's items, one stack per slot. plain class, the owner (Player) holds it
    public class Inventory
    {
        private readonly ItemStack[] _slots;    // null = empty slot
        public event Action<int, ItemStack> SlotChanged;

        public int Size => _slots.Length;

        public Inventory(int size)
        {
            _slots = new ItemStack[size];
        }

        public ItemStack GetStack(int index) => IsValid(index) ? _slots[index] : null;
        public IInventoryable Get(int index) => GetStack(index)?.Item;

        // put one item in the first slot that can hold it, a first slot could be:
        // 1. an empty slot
        // 2. a stack of the same item with room.
        public bool TryAdd(IInventoryable item)
        {
            if (item == null) return false;

            for (int i = 0; i < _slots.Length; i++)
            {
                // 1. an empty slot
                if (_slots[i] == null)
                {
                    Set(i, new ItemStack(item));
                    return true;
                }

                // 2. a stack of the same item with room.
                if (_slots[i].CanMerge(item))
                {
                    _slots[i].Count++;
                    Set(i, _slots[i]);
                    return true;
                }
            }

            return false;
        }

        // take "amount" off the stack. 
        public void Remove(int index, int amount = 1)
        {
            ItemStack stack = GetStack(index);
            if (stack == null) return;

            // stack is reduced
            stack.Count -= amount;

            // if this stack is 0, this stack is set to null
            Set(index, stack.Count > 0 ? stack : null);
        }

        // move the stack from slot to another slot, in the same inventory
        public void Move(int from, int to) => MoveBetweenInventory(this, from, this, to);

        // move the stack from slot to another slot, which may be in another inventory
        // e.g. backpack slot 2 -> hotbar slot 0
        // 1) same item with room => merge
        // 2) anything else       => the two slots swap
        public static void MoveBetweenInventory(Inventory a, int slotA, Inventory b, int slotB)
        {
            // guard
            if (!a.IsValid(slotA) || !b.IsValid(slotB)) return;
            if (a == b && slotA == slotB) return;

            ItemStack moving = a._slots[slotA];
            ItemStack target = b._slots[slotB];

            // merge
            if (moving != null && target != null && target.CanMerge(moving.Item))
            {
                int amount = Math.Min(moving.Count, target.Room);

                target.Count += amount;
                b.Set(slotB, target);
                a.Remove(slotA, amount);
                return;
            }

            // swap
            a.Set(slotA, target);
            b.Set(slotB, moving);
        }

        // ==================== private ====================
        private void Set(int index, ItemStack stack)
        {
            _slots[index] = stack;
            SlotChanged?.Invoke(index, stack);
        }

        private bool IsValid(int index) => index >= 0 && index < _slots.Length;
    }
}
