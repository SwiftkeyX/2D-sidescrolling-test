using SideScroller.Characters;
using UnityEngine;

namespace SideScroller.Inventories
{
    // FIXME: this is temporarily
    // ItemPickup work mainly 2 way: 
    // 1. to pickup item    - the item itself have ItemPickup. when player walk into these item lying in the world, 
    // the collision is trigger, that item was destroyed, and get put inside the inventory as SO.
    // 2. to drop item      - when dropping the item, the item is re-init itself from SO back into the world.
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup : MonoBehaviour
    {
        private const float FloorSearchDistance = 10f;
        // ================================= dependency =================================
        [SerializeField] private ScriptableObject _item;    // this item data
        [SerializeField] private SpriteRenderer _renderer;  // this item sprite
        private IInventoryable Item => _item as IInventoryable;

        // ================================= pick delay =================================
        // a dropped item waits a moment, so it doesn't jump straight back into the inventory
        [SerializeField] private float _pickupDelay = 1f;
        private float _pickableAt;

        // ================================= pickup item =================================
        // this item check for collision to player. if collided, player pickup this item.  
        void OnTriggerEnter2D(Collider2D other) => TryGiveTo(other);
        void OnTriggerStay2D(Collider2D other) => TryGiveTo(other);

        private void TryGiveTo(Collider2D other)
        {
            // item is pickable after a set delay
            if (Time.time < _pickableAt) return;

            // if not player, return
            Player player = other.GetComponentInParent<Player>();
            if (player == null) return;

            if (player.Inventory.TryAdd(Item)) Destroy(gameObject);
        }

        // ================================= drop item =================================
        // to drop an item into the world. 
        // this is a factory, for create a item from given item SO, and drop it to the world.
        public static ItemPickup Spawn(IInventoryable item, Vector2 position)
        {
            // guard
            if (FindDefaultPrefab() == null)
            {
                Debug.LogError($"[ItemPickup] no prefab at Resources/{DefaultPrefabPath}, so nothing can be spawned.");
                return null;
            }
            if (item is not ScriptableObject itemSO)
            {
                Debug.LogError($"[ItemPickup] {item?.DisplayName} is not an asset, so it cannot lie in the world.");
                return null;
            }

            ItemPickup pickup = Instantiate(FindDefaultPrefab(), position, Quaternion.identity);

            // init pickup delay
            pickup._pickableAt = Time.time + pickup._pickupDelay;

            // copy SO data e.g. sprite
            pickup._item = itemSO;
            pickup.name = item.DisplayName;
            pickup.ShowIcon();

            // place drop item on the floor
            pickup.PlaceOnFloor();

            return pickup;
        }

        // the pickup looks like the item it gives
        private void ShowIcon()
        {
            if (_renderer != null && Item != null) _renderer.sprite = Item.Icon;
        }

        // raycast down to the "Ground", then place the item there.
        private void PlaceOnFloor()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, FloorSearchDistance, LayerMask.GetMask("Ground"));
            if (hit.collider == null) return;

            float bottomToPivot = _renderer != null ? transform.position.y - _renderer.bounds.min.y : 0f;
            transform.position = new Vector3(transform.position.x, hit.point.y + bottomToPivot, transform.position.z);
        }

        // ================================= default prefab =================================
        // prefab for help spawning new ItemPickup. loaded from Assets/Resources/ItemPickup.prefab on first use.
        private const string DefaultPrefabPath = "ItemPickup";
        private static ItemPickup _defaultPrefab;
        private static ItemPickup FindDefaultPrefab()
        {
            if (_defaultPrefab == null) _defaultPrefab = Resources.Load<ItemPickup>(DefaultPrefabPath);
            return _defaultPrefab;
        }
        

        // ================================= life cycle =================================
        void Awake()
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            ShowIcon();
        }

        void OnValidate()
        {
            if (_item != null && Item == null)
            {
                Debug.LogWarning($"ItemPickup: '{_item.name}' is not an IInventoryable.", this);
                _item = null;
            }

            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();

            // ASKING: what is this
#if UNITY_EDITOR
            // unity forbids changing the renderer inside OnValidate, so wait one editor tick
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null) ShowIcon();
            };
#endif
        }
    }
}
