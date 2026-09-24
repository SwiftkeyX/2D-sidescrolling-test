using SideScroller.Characters;
using SideScroller.Input;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// The inventory window, centre of the overlay. a grid of cells, each cell show one item sprite.
    /// listen to the player's Inventory
    internal class InventoryPanelController : PanelController
    {
        private const string CellClass = "inventory__cell";
        private const string IconClass = "inventory__icon";

        [SerializeField] private Player _player;
        [SerializeField] private bool _startShown;

        private bool _shown;
        private Inventory _inventory;
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

        // open/close the window
        public void Toggle()
        {
            _shown = !_shown;
            SetShown(_shown);
        }

        // =================================== Life cycle ===================================
        protected override void OnMounted(VisualElement panel)
        {
            _shown = _startShown;
            SetShown(_shown);

            _grid = panel.Q<VisualElement>("InventoryGrid");
            if (_grid == null) return;

            if (_player == null) _player = FindFirstObjectByType<Player>();
            if (_player == null) return;

            _inventory = _player.Inventory;
            _inventory.SlotChanged += SetItem;

            BuildCells(_inventory.Size);
            InitItem();
        }

        void Update()
        {
            if (PlayerInputSystem.InventoryPressedThisFrame) Toggle();
        }

        void OnDisable()
        {
            if (_inventory != null) _inventory.SlotChanged -= SetItem;
        }

        // =================================== private ===================================
        private void BuildCells(int count)
        {
            _grid.Clear();
            _icons = new VisualElement[count];

            for (int i = 0; i < count; i++)
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

        // draw whatever the inventory already hold, e.g. the starting tool
        private void InitItem()
        {
            for (int i = 0; i < _inventory.Size; i++)
            {
                SetItem(i, _inventory.Get(i));
            }
        }
    }
}
