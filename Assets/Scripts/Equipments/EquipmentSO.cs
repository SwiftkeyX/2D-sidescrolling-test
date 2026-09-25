using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.Serialization;

namespace SideScroller.Equipments
{
    /// <summary>
    /// Data for one equippable thing
    /// </summary>
    [CreateAssetMenu(fileName = "Equipment", menuName = "SideScroller/Equipment")]
    public class EquipmentSO : ItemSO
    {
        [SerializeField] private EquipmentTypeEnum _type;
        [FormerlySerializedAs("_behaviourPrefab")]
        [SerializeField] private Equipment _equipment;

        public EquipmentTypeEnum Type => _type;
        public Equipment BehaviourPrefab => _equipment;

        // equipable things is categorized as tool
        public override ItemCategoryEnum Category => ItemCategoryEnum.Tool;

        // tools never stack
        public override int MaxStack => 1;
    }
}
