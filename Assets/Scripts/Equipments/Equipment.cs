using UnityEngine;

namespace SideScroller.Equipments
{
    public abstract class Equipment : MonoBehaviour
    {
        [SerializeField] private EquipmentSO _data;

        public EquipmentSO Data => _data;
        public EquipmentTypeEnum Type => _data == null ? default : _data.Type;

        public abstract void Activate(Vector2 direction);
    }
}
