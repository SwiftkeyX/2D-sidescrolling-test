using UnityEngine;

namespace SideScroller.Inventories
{
    /// Data for one item (lumber, carrot, storage chest...).
    /// Mother of every item data. children add what only their kind needs
    [CreateAssetMenu(fileName = "Item", menuName = "SideScroller/Item")]
    public class ItemSO : ScriptableObject, IInventoryable
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        // children may decide it themselves, 
        // e.g. EquipmentSO is always Tool
        [SerializeField] private ItemCategoryEnum _category;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public virtual ItemCategoryEnum Category => _category;
    }
}
