using UnityEngine;

namespace SideScroller.Equipments
{
    /// <summary>
    /// Tracks what the player is holding and drives the sprite shown above their head.
    /// It only shows the tool, what a tool does belongs to the tool.
    /// </summary>
    public class Equipment
    {
        private readonly SpriteRenderer _renderer;

        public EquipmentSO Current { get; private set; }
        public Sprite Icon => Current == null ? null : Current.Icon;
        public bool HasEquipment => Current != null;

        public Equipment(SpriteRenderer renderer)
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
