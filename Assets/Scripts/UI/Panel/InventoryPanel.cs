using SideScroller.Characters;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    // Inventory.cs is the logic part that keep the actual item list, and inventory panel display the result of that as UI.

    // Inventory panel also let the player arrange the item by moving the item sprite around
    // when that happen Inventory.cs is the one listen to the changed and change itself accordingly.
    internal abstract class InventoryPanelController : PanelController
    {
        private const string CellClass = "inventory__cell";
        private const string IconClass = "inventory__icon";

        [SerializeField] private Player _player;

        private VisualElement _grid;    // the containter for every cells
        private VisualElement[] _cells; // cell was ordered to create a grid visually. each cell contain icon
        private VisualElement[] _icons; // icon contain a item sprite

        // ============================= for children =============================
        protected Player Player => _player;
        protected Inventory Inventory { get; private set; }     // the data this panel shows
        protected VisualElement[] Cells => _cells;

        // this uxml name 
        // e.g. BackpackGrid, HotbarGrid
        protected abstract string GridName { get; }

        // context: inventory panel only show 1 inventory but player have 2 inventories = backpack, hotbar 
        // so this function only choose 1 of them to display 
        protected abstract Inventory PickInventory(Player player);

        // the parent init at mounted BUT the children still doesn't init their own things
        // this function let children init after the parent did init. 
        protected virtual void OnInventoryMounted(VisualElement panel) { }

        // =================================== public ===================================
        // context: The item is add/remove/moved around, Inventory panel need to display that item sprite is moved from cell to cell. 
        // this function put the item sprite inside the cell index
        public void SetItem(int index, IInventoryable item)
        {
            if (_icons == null || index < 0 || index >= _icons.Length) return;

            VisualElement icon = _icons[index];
            Sprite newSprite = item?.Icon;

            // copy the sprite to current icon
            icon.style.backgroundImage = newSprite == null ? new StyleBackground(StyleKeyword.None) : new StyleBackground(newSprite);

            // show tooltip
            icon.tooltip = item?.DisplayName;
        }

        // remove item sprite from the cell index
        public void ClearItem(int index) => SetItem(index, null);

        // =================================== Life cycle ===================================
        protected sealed override void OnMounted(VisualElement panel)
        {
            _grid = panel.Q<VisualElement>(GridName);
            if (_grid == null)
            {
                Debug.LogWarning($"{GetType().Name}: no element named '{GridName}' in the uxml, so this panel shows nothing.", this);
                return;
            }

            if (_player == null) _player = FindFirstObjectByType<Player>();
            if (_player == null)
            {
                Debug.LogWarning($"{GetType().Name}: no Player in the scene, so this panel shows nothing.", this);
                return;
            }

            Inventory = PickInventory(_player);
            Inventory.SlotChanged += SetItem;

            FindCells();
            InitItem();

            OnInventoryMounted(panel);
        }

        protected virtual void OnDisable()
        {
            if (Inventory != null) Inventory.SlotChanged -= SetItem;
        }

        // =================================== private ===================================
        // the cells are authored in the uxml, get those cells into code: 
        // Cell0 = slot 0, Cell1 = slot 1...
        private void FindCells()
        {
            _cells = _grid.Query<VisualElement>(className: CellClass).ToList().ToArray();
            _icons = new VisualElement[_cells.Length];

            for (int i = 0; i < _cells.Length; i++)
            {
                _icons[i] = _cells[i].Q<VisualElement>(className: IconClass);
            }

            // the uxml and the inventory should agree on how many slots there are
            if (_cells.Length != Inventory.Size)
            {
                Debug.LogWarning($"{GetType().Name}: uxml has {_cells.Length} cells but the inventory has {Inventory.Size} slots.");
            }
        }

        // init the item that this inventory already hold
        // e.g. the starting tool
        private void InitItem()
        {
            for (int i = 0; i < Inventory.Size; i++)
            {
                SetItem(i, Inventory.Get(i));
            }
        }
    }
}
