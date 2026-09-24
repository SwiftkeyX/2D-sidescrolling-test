using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// The inventory window, centre of the overlay. a grid of cells, each cell show one item sprite
    internal class InventoryPanelController : PanelController
    {
        private const string CellClass = "inventory__cell";
        private const string IconClass = "inventory__icon";

        [SerializeField] private int _cellCount = 10;

        // for testing only: must be assets implementing IInventoryable, drawn into the first cells
        [SerializeField] private ScriptableObject[] _previewItems;

        private VisualElement _grid;
        private VisualElement[] _icons;

        // =================================== public ===================================
        // put the item sprite inside the cell
        public void SetItem(int index, IInventoryable item)
        {
            if (_icons == null || index < 0 || index >= _icons.Length) return;

            VisualElement icon = _icons[index];
            Sprite sprite = item?.Icon;

            icon.style.backgroundImage = sprite == null ? new StyleBackground(StyleKeyword.None) : new StyleBackground(sprite);
            icon.tooltip = item?.DisplayName;
        }

        public void ClearItem(int index) => SetItem(index, null);

        // =================================== Life cycle ===================================
        protected override void OnMounted(VisualElement panel)
        {
            _grid = panel.Q<VisualElement>("InventoryGrid");
            if (_grid == null) return;

            BuildCells();
            ShowPreviewItems();
        }

        // =================================== private ===================================
        private void BuildCells()
        {
            _grid.Clear();
            _icons = new VisualElement[_cellCount];

            for (int i = 0; i < _cellCount; i++)
            {
                VisualElement cell = new VisualElement { name = $"Cell{i}" };
                cell.AddToClassList(CellClass);

                VisualElement icon = new VisualElement { pickingMode = PickingMode.Ignore };
                icon.AddToClassList(IconClass);

                cell.Add(icon);
                _grid.Add(cell);
                _icons[i] = icon;
            }
        }

        private void ShowPreviewItems()
        {
            if (_previewItems == null) return;

            for (int i = 0; i < _previewItems.Length; i++)
            {
                SetItem(i, _previewItems[i] as IInventoryable);
            }
        }
    }
}
