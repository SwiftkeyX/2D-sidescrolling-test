using SideScroller.Characters;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// The quick access bar, bottom-right of the overlay. always shown. 
    internal class HotbarPanelController : PanelController
    {
        private const string CellClass = "inventory__cell";
        private const string IconClass = "inventory__icon";

        [SerializeField] private Player _player;

        private Inventory _hotbar;
        private VisualElement _grid;    // the containter for every cells
        private VisualElement[] _cells; // cell was ordered to create a grid visually. each cell contain icon 
        private VisualElement[] _icons; // icon contain a item sprite

        // =================================== public ===================================
        // put the item sprite inside the cell
        public void SetItem(int index, IInventoryable item)
        {
            if (_icons == null || index < 0 || index >= _icons.Length) return;

            VisualElement icon = _icons[index];
            Sprite newSprite = item?.Icon;

            // copy the sprite to current icon
            icon.style.backgroundImage = newSprite == null ? new StyleBackground(StyleKeyword.None) : new StyleBackground(newSprite);
        }

        // =================================== Life cycle ===================================
        protected override void OnMounted(VisualElement panel)
        {
            _grid = panel.Q<VisualElement>("HotbarGrid");
            if (_grid == null) return;

            if (_player == null) _player = FindFirstObjectByType<Player>();
            if (_player == null) return;

            _hotbar = _player.Hotbar;
            _hotbar.SlotChanged += SetItem;

            FindCells();
            InitItem();
        }

        void OnDisable()
        {
            if (_hotbar != null) _hotbar.SlotChanged -= SetItem;
        }

        // =================================== private ===================================
        // the cells are authored in HotbarPanel.uxml, in slot order: Cell0 = slot 0, Cell1 = slot 1...
        private void FindCells()
        {
            _cells = _grid.Query<VisualElement>(className: CellClass).ToList().ToArray();
            _icons = new VisualElement[_cells.Length];

            for (int i = 0; i < _cells.Length; i++)
            {
                _icons[i] = _cells[i].Q<VisualElement>(className: IconClass);
            }

            // the uxml and the hotbar should agree on how many slots there are
            if (_cells.Length != _hotbar.Size)
            {
                Debug.LogWarning($"HotbarPanel: uxml has {_cells.Length} cells but the hotbar has {_hotbar.Size} slots.");
            }
        }

        // draw whatever the hotbar already hold, e.g. the starting tool
        private void InitItem()
        {
            for (int i = 0; i < _hotbar.Size; i++)
            {
                SetItem(i, _hotbar.Get(i));
            }
        }
    }
}
