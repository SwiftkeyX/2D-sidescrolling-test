using UnityEngine;

namespace SideScroller.Inventories
{
    // anything that can sit in an inventory cell. the cell only needs a sprite to draw
    public interface IInventoryable
    {
        string DisplayName { get; }
        Sprite Icon { get; }
        ItemCategoryEnum Category { get; }
    }
}
