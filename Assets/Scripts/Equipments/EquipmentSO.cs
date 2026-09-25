using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Equipments
{
    /// <summary>
    /// Data for one equippable thing
    /// </summary>
    [CreateAssetMenu(fileName = "Equipment", menuName = "SideScroller/Equipment")]
    public class EquipmentSO : ItemSO
    {
        [SerializeField] private EquipmentTypeEnum _type;
        [SerializeField] private Equipment _equipment;

        public EquipmentTypeEnum Type => _type;
        public Equipment BehaviourPrefab => _equipment;

        // equipable things is categorized as tool
        public override ItemCategoryEnum Category => ItemCategoryEnum.Tool;
    }
}
