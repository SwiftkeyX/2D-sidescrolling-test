using SideScroller.Characters;
using SideScroller.Crafting;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// display the opened crafting station's 3 craft cells, next to the backpack.
    /// works like the storage panel, but crafts:
    /// 1) E on a station = open it, together with the backpack so ingredients can be dragged in
    /// 2) E again, closing the backpack, or walking away = close it
    /// 3) preview + Craft button come from the mother (CraftPanel)
    internal class StationCraftPanel : CraftPanel
    {
        [SerializeField] private InventoryBackpackPanel _backpackPanel;
        [SerializeField] private float _closeDistance = 2.5f;     // walk further than this from the open station = it closes
        private CraftingStation _station;

        protected override string GridName => "StationGrid";
        protected override Inventory PickInventory(Player player) => null;

        // =================================== public ===================================
        // open the station
        public void Open(CraftingStation station)
        {
            _station = station;
            BindPanelToInventory(station.Grid);

            SetShown(true);
            _backpackPanel.SetShown(true);
        }

        // close the station, and the backpack that was opened with it
        public void Close()
        {
            _station = null;
            SetShown(false);
            _backpackPanel.SetShown(false);
        }

        // =================================== Life cycle ===================================
        protected override void OnCraftMounted(VisualElement panel)
        {
            if (_backpackPanel == null) _backpackPanel = FindFirstObjectByType<InventoryBackpackPanel>();

            CraftingStation.Interacted += OnStationInteracted;
            SetShown(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CraftingStation.Interacted -= OnStationInteracted;
        }

        void Update()
        {
            if (_station == null || Player == null) return;

            // when the player close backpack or the player walked away, close the station too
            if (!_backpackPanel.IsShown || !InReach(_station)) Close();
        }

        // =================================== private ===================================
        // E on the open station = close it, E on any other station = show that one
        private void OnStationInteracted(CraftingStation station)
        {
            if (station == _station) Close();
            else Open(station);
        }

        private bool InReach(CraftingStation station) =>
            station != null && Vector2.Distance(Player.transform.position, station.transform.position) <= _closeDistance;
    }
}
