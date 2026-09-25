
using SideScroller.Items;

namespace SideScroller.Characters
{
    /// part of the Player: left click uses the equipped tool.
    /// e.g. wand shoots, axe chops, watering can waters
    public partial class Player
    {
        // ======================================== combat ========================================
        // every tool activates the same way, the tool decides what that means
        public void ActivateTool()
        {
            // the equipped item's behaviour, spawned by the slot
            Equipment tool = _equipmentSlot.CurrentEquipment;
            if (tool == null) return;

            tool.Activate(GetAimDirection(), GetPointerWorldPosition());
        }
    }
}
