using SideScroller.Characters;
using SideScroller.Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    // This class, "DraggingModeController" is to control the dragging mode.
    // the dragging logic itself was in Dragging.cs

    // dragging item from inventory have 2 modes:
    // 1) backpack open   => arrange mode: 
    // - move items between backpack and hotbar 
    // - drag out to drop on the floor
    // 2) backpack closed => use mode: drag an item out of the hotbar to use it 
    // - equip a tool 
    // - plant a seed
    [RequireComponent(typeof(UIDocument))]
    internal class DraggingModeController : MonoBehaviour
    {
        private const string GhostClass = "inventory__ghost";

        [SerializeField] private Player _player;
        [SerializeField] private InventoryBackpackPanel _backpackPanel;
        [SerializeField] private InventoryHotbarPanel _hotbarPanel;
        [SerializeField] private StyleSheet _ghostStyle;

        private VisualElement _ghost;
        private Dragging _arrange;
        private Dragging _use;

        // =================================== Life cycle ===================================
        void Update()
        {
            // guard
            if (_arrange == null && !TryBuild()) return;

            // when backpack is open, arrange mode
            // when backpack is closed, use mode
            Dragging active = _backpackPanel.IsShown ? _arrange : _use;
            Dragging other = active == _arrange ? _use : _arrange;

            // the mode just switched mid-drag, put the item back
            if (other != null && other.IsHolding) other.Cancel();

            active?.Tick();

            // lock player's tool when:
            // 1) player's backpack is open
            // 2) player's pointer on the hotbar
            // 3) player is dragging an item
            _player.ToolsLocked = _backpackPanel.IsShown || IsPointerOnHotbar() || (active != null && active.IsHolding);
        }

        void OnDisable()
        {
            if (_player != null) _player.ToolsLocked = false;

            _arrange?.Cancel();
            _use?.Cancel();
            _ghost?.RemoveFromHierarchy();

            _arrange = null;
            _use = null;
            _ghost = null;
        }

        // ======================================== init ========================================
        /// <summary>
        /// init this class
        /// init dependency, ghost, 2 mode of dragging funtion
        /// </summary>
        private bool TryBuild()
        {
            // init dependency
            if (_player == null) _player = FindFirstObjectByType<Player>();
            if (_backpackPanel == null) _backpackPanel = FindFirstObjectByType<InventoryBackpackPanel>();
            if (_hotbarPanel == null) _hotbarPanel = FindFirstObjectByType<InventoryHotbarPanel>();

            // basic guard
            if (_player == null || _backpackPanel == null || _backpackPanel.Inventory == null) return false;
            if (_hotbarPanel != null && _hotbarPanel.Inventory == null) return false;

            // basic guard + send backpack & hotbar to InventoryPanel
            InventoryPanelController[] arrangePanels = _hotbarPanel == null
                ? new InventoryPanelController[] { _backpackPanel }
                : new InventoryPanelController[] { _backpackPanel, _hotbarPanel };

            // init ghost
            _ghost = BuildGhost();

            // init arrange mode
            _arrange = new Dragging(
                panels: arrangePanels,  // arrange mode need both backpack & hotbar 
                ghost: _ghost,
                canRearrange: true,
                releasedOutside: _player.DropItem   // dropping outside mean dropping item on the floor
            );

            // init use mode 
            _use = new Dragging(
                panels: new InventoryPanelController[] { _hotbarPanel }, // use mode only wanat hotbar
                ghost: _ghost,
                canRearrange: false,
                releasedOutside: _player.UseItem    // dropping outside mean to use item
            );

            return true;
        }

        // context: ghost = the VisualElement that copy style from the item sprite. and can follow player's pointer.
        // when player hold on item, the ghost was created to follow player's pointer, create a visually dragged item.
        // this function is to init the ghost to the panel.
        private VisualElement BuildGhost()
        {
            // ignore so the pointer doesn't pick up the ghost when it don't want to
            VisualElement ghost = new VisualElement { pickingMode = PickingMode.Ignore };

            // init uss
            ghost.AddToClassList(GhostClass);
            ghost.style.display = DisplayStyle.None;
            if (_ghostStyle != null) ghost.styleSheets.Add(_ghostStyle);
            else Debug.LogWarning("InventoryDragController: no Ghost Style set, the dragged icon will not show.", this);

            // add ghost to the UI document
            GetComponent<UIDocument>().rootVisualElement.Add(ghost);

            return ghost;
        }

        // ======================================== helper ========================================
        // is the pointer over the hotbar? 
        private bool IsPointerOnHotbar()
        {
            if (_hotbarPanel == null || _ghost?.panel == null) return false;

            Vector2 panelPos = Picker.ToPanel(_ghost.panel, PlayerInputSystem.PointerScreenPosition);
            return _hotbarPanel.IsPointerInside(panelPos);
        }

    }
}
