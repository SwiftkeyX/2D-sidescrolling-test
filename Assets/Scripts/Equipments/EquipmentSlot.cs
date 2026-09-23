using UnityEngine;

namespace SideScroller.Equipments
{
    // player have 1 equipment slot which the sprite is show above player's head
    // EquipmentSlot hold the logic for swapping that sprite for the player
    public class EquipmentSlot
    {
        private readonly SpriteRenderer _renderer;

        public EquipmentSO Current { get; private set; }
        public Sprite Icon => Current == null ? null : Current.Icon;
        public bool HasEquipment => Current != null;

        public EquipmentSlot(SpriteRenderer renderer)
        {
            _renderer = renderer;
            Refresh();
        }

        public void Equip(EquipmentSO equipment)
        {
            Current = equipment;
            Refresh();
        }

        public void Unequip()
        {
            Current = null;
            Refresh();
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
