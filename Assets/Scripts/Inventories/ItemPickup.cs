using SideScroller.Characters;
using UnityEngine;

namespace SideScroller.Inventories
{
    // FIXME: this is temporarily
    /// An item lying in the world. Walk into it and it goes into the player's first empty slot.
    /// If the inventory is full, it stays on the ground.
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup : MonoBehaviour
    {
        // must be an asset implementing IInventoryable (ItemSO, EquipmentSO)
        [SerializeField] private ScriptableObject _item;
        [SerializeField] private SpriteRenderer _renderer;

        private IInventoryable Item => _item as IInventoryable;

        void Awake()
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            ShowIcon();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // if not player, return
            Player player = other.GetComponentInParent<Player>();
            if (player == null) return;

            if (player.Inventory.TryAdd(Item)) Destroy(gameObject);
        }

        void OnValidate()
        {
            if (_item != null && Item == null)
            {
                Debug.LogWarning($"ItemPickup: '{_item.name}' is not an IInventoryable.", this);
                _item = null;
            }

            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();

#if UNITY_EDITOR
            // unity forbids changing the renderer inside OnValidate, so wait one editor tick
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null) ShowIcon();
            };
#endif
        }

        // the pickup looks like the item it gives
        private void ShowIcon()
        {
            if (_renderer != null && Item != null) _renderer.sprite = Item.Icon;
        }
    }
}
