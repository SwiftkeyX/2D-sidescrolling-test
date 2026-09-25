using System;
using System.Collections.Generic;
using SideScroller.Inventories;
using SideScroller.Items;
using UnityEngine;

namespace SideScroller.Crafting
{
    /// This class, "Craft" is to check if the ingredient match the correct ingredient.
    /// the order of items in the craft cells doesn't matter.
    [Serializable]
    public class Recipe
    {
        // ingredient for this recipe e.g. Lumber x10
        [SerializeField] private List<Ingredient> _ingredients = new();

        // result for this recipe e.g. Storage Chest
        [SerializeField] private ItemSO _result;

        public IReadOnlyList<Ingredient> Ingredients => _ingredients;
        public ItemSO Result => _result;

        // ======================================== public ========================================
        // the grid matches when:
        // 1) there's no extra ingredient that wasn't in recipe
        // 2) it has at least the amount of every ingredient
        // e.g. Golden Veggie recipe: need = {Carrot:1, Red Berry:1, Corn:1}
        //   cells [Corn][Carrot][Red Berry] => have = {Corn:1, Carrot:1, Red Berry:1} => match (the dictionary has no order)
        //   cells [Lumber x10][Carrot]      => Carrot isn't in the chest recipe      => fails 1)
        //   cells [Lumber x9]               => 9 < 10                                => fails 2)
        public bool Matches(Inventory grid)
        {
            Dictionary<IInventoryable, int> have = CountByItem(grid);
            Dictionary<IInventoryable, int> need = Needs();

            // empty grid, or a recipe with no ingredients, never matches
            if (have.Count == 0 || need.Count == 0) return false;

            // 1) there's no extra ingredient that wasn't in recipe
            foreach (KeyValuePair<IInventoryable, int> item in have)
            {
                if (!need.ContainsKey(item.Key)) return false;
            }

            // 2) there's no ingredient missing, and enough of each
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
        // get dictionary of what this recipe need
        // e.g. Storage Chest need {Lumber: 10}
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

        // get dictionary of what the grid current hold
        // e.g. [Lumber x5][Lumber x5][-]   => {Lumber:10}
        //      [Corn x1][-][Carrot x1]  => {Corn:1, Carrot:1}
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
