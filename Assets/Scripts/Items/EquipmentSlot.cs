using UnityEngine;

namespace SideScroller.Items
{
    // player have 1 equipment slot which the sprite is show above player's head
    // EquipmentSlot hold the logic for swapping that sprite for the player
    public class EquipmentSlot
    {
        private readonly SpriteRenderer _renderer;
        private readonly Transform _holder;     // the spawned behaviour is parented here

        public EquipmentSO SO { get; private set; }
        public Equipment CurrentEquipment { get; private set; }
        public Sprite Icon => SO == null ? null : SO.Icon;
        public bool HasEquipment => SO != null;

        public EquipmentSlot(SpriteRenderer renderer, Transform holder)
        {
            _renderer = renderer;
            _holder = holder;
            Refresh();
        }

        public void Equip(EquipmentSO equipment)
        {
            // re-equipping the same tool keeps its behaviour
            if (equipment == SO) return;

            DespawnTool();
            SO = equipment;
            SpawnTool();
            Refresh();
        }

        public void Unequip()
        {
            DespawnTool();
            SO = null;
            Refresh();
        }

        // switch current equipment to set SO, and put it on the head
        private void SpawnTool()
        {
            if (SO == null || SO.BehaviourPrefab == null) return;

            CurrentEquipment = Object.Instantiate(SO.BehaviourPrefab, _holder);
            CurrentEquipment.transform.localPosition = Vector3.zero;
        }

        // destroy current equipment
        private void DespawnTool()
        {
            if (CurrentEquipment != null) Object.Destroy(CurrentEquipment.gameObject);
            CurrentEquipment = null;
        }

        // renderer is switched off with empty hands, the anchor keeps following the player either way
        private void Refresh()
        {
            if (_renderer == null) return;

            _renderer.sprite = Icon;
            _renderer.enabled = HasEquipment;
        }
    }
}
