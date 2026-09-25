using System;
using UnityEngine;

namespace SideScroller.Inventories
{
    // this is for pre-author the item, specficially inside inspector.
    // an item and its count, authored in the Inspector
    // e.g. a chest's starting items: Lumber x10, Carrot Seed x3

    // This may look like ItemStack but they serve 2 different purpose:
    // ItemStack hold IInventory and amount which is similar to ItemAmount. 
    // But ItemStack is to put the stack of item inside the inventory which need interface Iinventory to do so.
    // Because ItemStack hold interface, so the field won't show in the inspector.
    // In summary, better create new class for this.
    [Serializable]
    public class ItemAmount
    {
        [SerializeField] private ItemSO _item;
        [SerializeField, Min(1)] private int _count = 1;

        public ItemSO Item => _item;
        public int Count => _count;
    }
}
