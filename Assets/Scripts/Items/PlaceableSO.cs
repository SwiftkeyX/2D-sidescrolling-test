using UnityEngine;

namespace SideScroller.Items
{
    /// <summary>
    /// Data for an item that can be put down in the world 
    /// e.g. Storage Chest.
    /// </summary>
    [CreateAssetMenu(fileName = "Placeable", menuName = "SideScroller/Placeable")]
    public class PlaceableSO : ItemSO
    {
        private const float GroundSearchDistance = 4f;

        [SerializeField] private GameObject _prefab;

        public GameObject Prefab => _prefab;

        // spawn the prefab standing on the ground below "near".
        public GameObject TryPlace(Vector2 near)
        {
            if (_prefab == null) return null;

            // find the ground below the pointer
            RaycastHit2D ground = Physics2D.Raycast(near + Vector2.up * 0.5f, Vector2.down, GroundSearchDistance, LayerMask.GetMask("Ground"));
            if (ground.collider == null) return null;

            // init the object
            GameObject placed = Instantiate(_prefab, ground.point, Quaternion.identity);
            placed.name = DisplayName;

            // place the sprite's bottom edge on the ground
            SpriteRenderer sprite = placed.GetComponentInChildren<SpriteRenderer>();
            if (sprite != null)
            {
                Vector3 pos = placed.transform.position;
                float bottomToPivot = pos.y - sprite.bounds.min.y;
                placed.transform.position = new Vector3(pos.x, ground.point.y + bottomToPivot, pos.z);
            }

            return placed;
        }
    }
}
