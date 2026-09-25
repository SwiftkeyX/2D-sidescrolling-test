using System;
using System.Collections.Generic;
using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Crafting
{
    /// One combination: up to 3 ingredients => a result.
    /// the order of items in the craft cells doesn't matter:
    /// the grid is counted by item, e.g. {Carrot:1, Corn:1, Red Berry:1}, then compared to what the recipe needs.
    /// Craft mechanic exist in this game. 
    /// This class, "Craft" is to check if the ingredient match the correct
    [Serializable]
    public class Craft
    {
        [SerializeField] private List<Ingredient> _ingredients = new();
        [SerializeField] private ItemSO _result;

        public IReadOnlyList<Ingredient> Ingredients => _ingredients;
        public ItemSO Result => _result;

        // ======================================== public ========================================
        // the grid matches when:
        // 1) it has at least the amount of every ingredient
        // 2) it has nothing that isn't an ingredient
        // FLAGGING: I don't understand this. we'll come back later.
        public bool Matches(Inventory grid)
        {
            Dictionary<IInventoryable, int> have = CountByItem(grid);
            Dictionary<IInventoryable, int> need = Needs();

            if (have.Count == 0 || need.Count == 0) return false;

            foreach (KeyValuePair<IInventoryable, int> item in have)
            {
                if (!need.ContainsKey(item.Key)) return false;
            }

            foreach (KeyValuePair<IInventoryable, int> item in need)
            {
                if (!have.TryGetValue(item.Key, out int count) || count < item.Value) return false;
            }

            return true;
        }

        // take exactly the ingredient amounts out of the grid, whatever is left over stays in its cell
        // e.g. Lumber x12 => the chest uses 10, Lumber x2 stays
        public void Consume(Inventory grid)
        {
            foreach (KeyValuePair<IInventoryable, int> item in Needs())
            {
                int remaining = item.Value;

                for (int i = 0; i < grid.Size && remaining > 0; i++)
                {
                    ItemStack stack = grid.GetStack(i);
                    if (stack == null || stack.Item != item.Key) continue;

                    int take = Math.Min(stack.Count, remaining);
                    grid.Remove(i, take);
                    remaining -= take;
                }
            }
        }

        // ======================================== private ========================================
        // what the recipe asks for, by item.
        // FIXME: put a example here for clarification Needs = (Lumber, 10), etc...
        private Dictionary<IInventoryable, int> Needs()
        {
            Dictionary<IInventoryable, int> need = new();

            foreach (Ingredient ingredient in _ingredients)
            {
                if (ingredient.Item == null) continue;

                need.TryGetValue(ingredient.Item, out int count);
                need[ingredient.Item] = count + ingredient.Count;
            }

            return need;
        }

        // what the grid holds, by item. the same item split over 2 cells is added together
        // FIXME: put a example here for clarification Needs = (Lumber, 10), etc...
        private static Dictionary<IInventoryable, int> CountByItem(Inventory grid)
        {
            Dictionary<IInventoryable, int> have = new();

            for (int i = 0; i < grid.Size; i++)
            {
                ItemStack stack = grid.GetStack(i);
                if (stack == null) continue;

                have.TryGetValue(stack.Item, out int count);
                have[stack.Item] = count + stack.Count;
            }

            return have;
        }
    }
}
