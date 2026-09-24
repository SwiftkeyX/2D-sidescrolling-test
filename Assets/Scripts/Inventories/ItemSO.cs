using UnityEngine;

namespace SideScroller.Inventories
{
    /// <summary>
    /// Data for one plain item (lumber, carrot, seed...). A new item is a new asset, not new code.
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "SideScroller/Item")]
    public class ItemSO : ScriptableObject, IInventoryable
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
    }
}
