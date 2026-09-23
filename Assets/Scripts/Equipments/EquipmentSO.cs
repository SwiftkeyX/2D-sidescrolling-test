using UnityEngine;

namespace SideScroller.Equipments
{
    /// <summary>
    /// Data for one equippable thing. A new tool is a new asset, not new code.
    /// </summary>
    [CreateAssetMenu(fileName = "Equipment", menuName = "SideScroller/Equipment")]
    public class EquipmentSO : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private EquipmentTypeEnum _type;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public EquipmentTypeEnum Type => _type;
    }
}
