namespace SideScroller.Inventories
{
    // what kind of item it is. decides what happens when the item is used from the hotbar
    // Resource = does nothing, used for crafting    e.g. lumber, carrot
    // Tool     = equip it, left click to use it     e.g. wand, axe
    // Seed     = plant it on the ground             e.g. carrot seed
    // Crafted  = made by crafting                   e.g. storage chest
    public enum ItemCategoryEnum { Resource, Tool, Seed, Crafted }
}
