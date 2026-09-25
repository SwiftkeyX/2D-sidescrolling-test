using SideScroller.Characters;
using SideScroller.Input;
using SideScroller.Inventories;
using SideScroller.Storage;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// display the opened storage chest as a panel, next to the backpack.
    /// functionality on top of mother:
    /// 1) E near a chest = open it, together with the backpack so items can be moved between them
    /// 2) E again, closing the backpack, or walking away = close it
    /// 3) drag to re-arrange, same as the backpack 
    internal class InventoryStoragePanel : InventoryPanel
    {
        [SerializeField] private InventoryBackpackPanel _backpackPanel;
        [SerializeField] private float _reach = 1.5f;      // how close the player has to stand to open a chest
        private StorageChest _chest;

        protected override string GridName => "StorageGrid";
        protected override Inventory PickInventory(Player player) => null;

        // =================================== public ===================================
        // open the storage
        public void Open(StorageChest chest)
        {
            _chest = chest;
            BindPanelToInventory(chest.Contents);

            SetShown(true);
            _backpackPanel.SetShown(true);
        }

        // close the storage
        public void Close()
        {
            _chest = null;
            SetShown(false);
        }

        // =================================== Life cycle ===================================
        protected override void OnInventoryMounted(VisualElement panel)
        {
            if (_backpackPanel == null) _backpackPanel = FindFirstObjectByType<InventoryBackpackPanel>();

            SetShown(false);
        }

        void Update()
        {
            if (Player == null || _backpackPanel == null) return;

            // when the player close backpack or the player walked away, close the storage too
            if (_chest != null && (!_backpackPanel.IsShown || !InReach(_chest)))
            {
                Close();
                _backpackPanel.SetShown(false);
            }

            // polling for interact input
            if (!PlayerInputSystem.InteractPressedThisFrame) return;
            bool isThisStorageBindToInventory = (_chest != null);

            // if this storage is bind, close the storage window
            if (isThisStorageBindToInventory)
            {
                Close();
                _backpackPanel.SetShown(false);
                return;
            }
            // if this storage is NOT bind, open the storage window
            else
            {
                StorageChest nearest = StorageChest.FindNearest(Player.transform.position, _reach);
                if (nearest != null) Open(nearest);
            }
        }

        private bool InReach(StorageChest chest) =>
            chest != null && Vector2.Distance(Player.transform.position, chest.transform.position) <= _reach;
    }
}
