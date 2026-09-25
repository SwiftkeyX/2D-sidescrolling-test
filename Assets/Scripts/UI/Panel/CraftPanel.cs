using SideScroller.Crafting;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// the mother of every craft window: 
    /// craft cells => result preview => Craft button.
    /// 0) the craft cells are a normal inventory, so dragging/stacking work like the backpack.
    /// 1) whenever the cells change, preview what the recipe result
    /// 2) when craft, use the ingredients up, the result goes into the backpack

    /// There's 2 kind of craft panel:
    /// 1) backpack - when player open backpack, player have access to 2 cell of craft panel
    /// 2) stationary - when player open backpack, player have access to 3 cell of craft panel
    internal abstract class CraftPanel : InventoryPanel
    {
        private const string ResultIconName = "ResultIcon";
        private const string CraftButtonName = "CraftButton";

        private VisualElement _resultIcon;
        private Button _craftButton;

        // =================================== Life cycle ===================================
        protected sealed override void OnInventoryMounted(VisualElement panel)
        {
            _resultIcon = panel.Q<VisualElement>(ResultIconName);
            _craftButton = panel.Q<Button>(CraftButtonName);
            if (_craftButton != null) _craftButton.clicked += OnCraftClicked;

            OnCraftMounted(panel);
            ShowResult();
        }

        // children init after the craft part did
        protected virtual void OnCraftMounted(VisualElement panel) { }

        // whenever the cells change, preview what the recipe result
        protected override void OnInventoryChanged() => ShowResult();

        // =================================== private ===================================
        private void OnCraftClicked()
        {
            if (Player == null || Inventory == null) return;

            Player.TryCraft(Inventory);
        }

        // preview what the recipe result
        private void ShowResult()
        {
            if (_resultIcon == null) return;

            Craft recipe = Player == null || Inventory == null ? null : Player.LookupRecipe(Inventory);
            
            // if the recipe is matched something, show the result icon
            Sprite icon = recipe?.Result == null ? null : recipe.Result.Icon;
            _resultIcon.style.backgroundImage = icon == null ? new StyleBackground(StyleKeyword.None) : new StyleBackground(icon);

            // set craft button enable/disable 
            _craftButton?.SetEnabled(recipe != null);
        }
    }
}
