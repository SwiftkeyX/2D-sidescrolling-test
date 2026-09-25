using SideScroller.Characters;
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
        [SerializeField] private float _closeDistance = 2.5f;     // walk further than this from the open chest = it closes
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

        // close the storage, and the backpack that was opened with it
        public void Close()
        {
            _chest = null;
            SetShown(false);
            _backpackPanel.SetShown(false);
        }

        // =================================== Life cycle ===================================
        protected override void OnInventoryMounted(VisualElement panel)
        {
            if (_backpackPanel == null) _backpackPanel = FindFirstObjectByType<InventoryBackpackPanel>();

            StorageChest.Interacted += OnChestInteracted;
            SetShown(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StorageChest.Interacted -= OnChestInteracted;
        }

        void Update()
        {
            if (_chest == null || Player == null) return;

            // when the player close backpack or the player walked away, close the storage too
            if (!_backpackPanel.IsShown || !InReach(_chest)) Close();
        }

        // =================================== private ===================================
        // E on the open chest = close it, E on any other chest = show that one
        private void OnChestInteracted(StorageChest chest)
        {
            if (chest == _chest) Close();
            else Open(chest);
        }

        private bool InReach(StorageChest chest) =>
            chest != null && Vector2.Distance(Player.transform.position, chest.transform.position) <= _closeDistance;
    }
}
