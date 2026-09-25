using System.Collections.Generic;
using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Crafting
{
    /// Every recipe in the game is list here.
    /// craft mechanic ask this book what their grid makes.
    [CreateAssetMenu(fileName = "RecipeBook", menuName = "SideScroller/Recipe Book")]
    public class RecipeBookSO : ScriptableObject
    {
        // all recipe list in here
        [SerializeField] private List<Craft> _recipes = new();
        public IReadOnlyList<Craft> Recipes => _recipes;

        // lookup the recipe this grid makes.
        // FLAGGING: if 2 recipes match, the first one in the list wins
        public Craft LookupRecipe(Inventory grid)
        {
            foreach (Craft recipe in _recipes)
            {
                if (recipe.Matches(grid)) return recipe;
            }

            return null;
        }

        // craft the specifcy ingredient
        public bool TryCraft(Inventory grid, Inventory output)
        {
            // lookup recipe
            Craft recipe = LookupRecipe(grid);

            // the recipe is incorrect, return
            if (recipe == null || recipe.Result == null) return false;

            // the inventory is full, return
            if (!output.CanAdd(recipe.Result)) return false;

            // the craft is success
            output.TryAdd(recipe.Result);

            // consume ingredient
            recipe.Consume(grid);

            return true;
        }
    }
}
