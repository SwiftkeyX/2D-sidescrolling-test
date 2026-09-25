using SideScroller.Inventories;
using SideScroller.Items;
using UnityEngine;

namespace SideScroller.Woodcutting
{
    /// A tree standing in the world. can be chopped by the axes.
    [RequireComponent(typeof(Collider2D))]
    public class ChoppableTree : MonoBehaviour
    {
        [SerializeField] private int _hitsToCut = 3;
        [SerializeField] private ItemSO _drop;              // what the tree drops, e.g. Lumber
        [SerializeField] private int _dropCount = 3;
        [SerializeField] private float _dropSpread = 0.6f;  // gap between each dropped item

        private int _hitsLeft;

        // ================================= 1. chop =================================
        public void Chop()
        {
            if (_hitsLeft <= 0) return;

            _hitsLeft--;
            if (_hitsLeft == 0) Fall();
        }

        // ================================= 2. fall =================================
        // drop the items in a row around the trunk, ItemPickup puts each one on the floor
        private void Fall()
        {
            if (_drop != null)
            {
                Vector2 trunk = (Vector2)transform.position + Vector2.up * 0.5f;
                float first = -(_dropCount - 1) * _dropSpread / 2f;

                for (int i = 0; i < _dropCount; i++)
                {
                    ItemPickup.Spawn(_drop, trunk + new Vector2(first + i * _dropSpread, 0f));
                }
            }

            Destroy(gameObject);
        }

        void Awake()
        {
            _hitsLeft = _hitsToCut;
        }
    }
}
