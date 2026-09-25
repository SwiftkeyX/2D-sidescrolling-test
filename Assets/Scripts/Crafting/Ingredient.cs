using System;
using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Crafting
{
    [Serializable]
    public class Ingredient
    {
        [SerializeField] private ItemSO _item;
        [SerializeField, Min(1)] private int _count = 1;

        public ItemSO Item => _item;
        public int Count => _count;
    }
}
