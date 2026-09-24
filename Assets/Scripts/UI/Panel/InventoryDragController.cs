using SideScroller.Characters;
using SideScroller.Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    // allow dragging item from inventory. Dragging to achieve:
    // 1) re-arrage item
    // 2) dragging to drop item
    // 3) (not imlemented) dragging to activate item
    [RequireComponent(typeof(UIDocument))]
    internal class InventoryDragController : MonoBehaviour
    {
        private const string GhostClass = "inventory__ghost";

        [SerializeField] private Player _player;
        [SerializeField] private InventoryBackpackPanel _backpackPanel;
        [SerializeField] private InventoryHotbarPanel _hotbarPanel;
        [SerializeField] private StyleSheet _ghostStyle;    

        private VisualElement _ghost;       
        private InventoryDragging _dragging;

        // =================================== Life cycle ===================================
        void Update()
        {
            // guard
            if (_dragging == null) _dragging = TryBuild();
            if (_dragging == null) return;

            // lock player's tool when:
            // 1) player's backpack is open
            // 2) player's pointer on the hotbar
            _player.ToolsLocked = _backpackPanel.IsShown || IsPointerOnHotbar();

            // if backpack open, ticking to allow re-arrange, drop
            if (_backpackPanel.IsShown)
            {
                _dragging.Tick();
                return;
            }

            // if backpack closed mid-drag, put the item back
            if (_dragging.IsHolding) _dragging.Cancel();
        }

        void OnDisable()
        {
            if (_player != null) _player.ToolsLocked = false;

            _dragging?.Cancel();
            _ghost?.RemoveFromHierarchy();

            _dragging = null;
            _ghost = null;
        }

        // =================================== private ===================================
        // built on first Update, not in OnEnable: the panels mount in their own OnEnable, which may run after this one.
        // null = not ready yet, try again next frame
        private InventoryDragging TryBuild()
        {
            if (_player == null) _player = FindFirstObjectByType<Player>();
            if (_backpackPanel == null) _backpackPanel = FindFirstObjectByType<InventoryBackpackPanel>();
            if (_hotbarPanel == null) _hotbarPanel = FindFirstObjectByType<InventoryHotbarPanel>();

            // a panel is ready once it has its Inventory
            if (_player == null || _backpackPanel == null || _backpackPanel.Inventory == null) return null;
            if (_hotbarPanel != null && _hotbarPanel.Inventory == null) return null;

            InventoryPanelController[] panels = _hotbarPanel == null
                ? new InventoryPanelController[] { _backpackPanel }
                : new InventoryPanelController[] { _backpackPanel, _hotbarPanel };

            _ghost = BuildGhost();
            return new InventoryDragging(_player, panels, _ghost);
        }

        // is the pointer over the hotbar? 
        private bool IsPointerOnHotbar()
        {
            if (_hotbarPanel == null || _ghost?.panel == null) return false;

            Vector2 panelPos = Picker.ToPanel(_ghost.panel, PlayerInputSystem.PointerScreenPosition);
            return _hotbarPanel.IsPointerInside(panelPos);
        }

        // context: ghost = the icon that copy style from the item sprite. and can move follow player's pointer.
        // it was to make a item sprite that can be dragged around by the player.
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
    }
}
