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
        private const string GhostClass = "inventory__ghost";

        [SerializeField] private Player _player;
        [SerializeField] private bool _startShown;

        private bool _shown;
        private Inventory _inventory;
        private VisualElement _grid;    // the containter for every cells
        private VisualElement[] _cells; // cell contain 
        private VisualElement[] _icons; // icon
        private VisualElement _ghost;
        private InventoryDragging _dragging;

        // =================================== public ===================================
        // put the item sprite inside the cell
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

        public void ClearItem(int index) => SetItem(index, null);

        // open/close the window
        public void Toggle()
        {
            _shown = !_shown;
            SetShown(_shown);

            // closing the window mid-drag put the item back
            if (!_shown) _dragging?.Cancel();
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

            FindCells();
            InitItem();

            _ghost = BuildGhost();
            _dragging = new InventoryDragging(_inventory, _player, panel, _cells, _ghost);
        }

        void Update()
        {
            if (PlayerInputSystem.InventoryPressedThisFrame) Toggle();

            if (_shown) _dragging?.Tick();
        }

        void OnDisable()
        {
            if (_inventory != null) _inventory.SlotChanged -= SetItem;

            _dragging?.Cancel();
            _ghost?.RemoveFromHierarchy();
        }

        // =================================== private ===================================
        // the cells are authored in InventoryPanel.uxml, in slot order: Cell0 = slot 0, Cell1 = slot 1...
        private void FindCells()
        {
            _cells = _grid.Query<VisualElement>(className: CellClass).ToList().ToArray();
            _icons = new VisualElement[_cells.Length];

            for (int i = 0; i < _cells.Length; i++)
            {
                _icons[i] = _cells[i].Q<VisualElement>(className: IconClass);
            }

            // the uxml and the inventory should agree on how many slots there are
            if (_cells.Length != _inventory.Size)
            {
                Debug.LogWarning($"InventoryPanel: uxml has {_cells.Length} cells but the inventory has {_inventory.Size} slots.");
            }
        }

        // the dragged icon
        private VisualElement BuildGhost()
        {
            VisualElement ghost = new VisualElement { pickingMode = PickingMode.Ignore };
            ghost.AddToClassList(GhostClass);
            ghost.style.display = DisplayStyle.None;

            // FIXLATER: This is kinda messy. It happen because the .uss isn't organize.
            // the ghost lives outside this panel, so it doesn't inherit the panel's .uss. hand it the same sheets
            for (int i = 0; i < Panel.styleSheets.count; i++)
            {
                ghost.styleSheets.Add(Panel.styleSheets[i]);
            }

            MainPanel.Add(ghost);
            return ghost;
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
