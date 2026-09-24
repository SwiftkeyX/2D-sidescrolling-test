using UnityEngine;

namespace SideScroller.Inventories
{
    /// Data for one plain item (lumber, carrot, seed...). 
    [CreateAssetMenu(fileName = "Item", menuName = "SideScroller/Item")]
    public class ItemSO : ScriptableObject, IInventoryable
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
    }
}
