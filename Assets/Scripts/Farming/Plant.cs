using SideScroller.Characters;
using SideScroller.Interactions;
using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Farming
{
    /// A seed planted on the ground. all seeds act the same.
    /// the SeedSO.cs decides the look and when to harvest:
    /// 1) planted near the pointer, standing on the ground
    /// 2) each watering grows it one stage
    /// 3) fully grown + player presses E on it = the harvest goes into the backpack, 3 seeds drop on the ground, the plant is gone
    [RequireComponent(typeof(Collider2D))]
    public class Plant : MonoBehaviour, IInteractable
    {
        private const string PrefabPath = "Plant";          // path to /Resource and loaded into scene
        private const float SpaceCheckRadius = 0.4f;        // one plant per spot
        private const float GroundSearchDistance = 4f;
        private const float SeedDropSpread = 0.5f;          // gap between the seeds dropped on harvest

        [SerializeField] private SpriteRenderer _renderer;

        private SeedSO _seed;
        private int _stage;

        public bool IsGrown => _seed != null && _stage >= _seed.GrownStage;

        // ================================= 1. planted =================================
        // this is factory for create seed. it is to plant the seed on the ground below "near".
        public static Plant TryPlant(SeedSO seed, Vector2 near)
        {
            if (seed == null || seed.StageCount == 0) return null;

            // find the ground below the pointer. 
            RaycastHit2D ground = Physics2D.Raycast(near + Vector2.up * 0.5f, Vector2.down, GroundSearchDistance, LayerMask.GetMask("Ground"));
            if (ground.collider == null) return null;

            // one plant per spot
            // check with specify radius
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(ground.point + Vector2.up * 0.5f, SpaceCheckRadius))
            {
                if (hit.GetComponent<Plant>() != null) return null;
            }

            // load prefab from /Resources
            Plant prefab = Resources.Load<Plant>(PrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"[Plant] no prefab at Resources/{PrefabPath}, so nothing can be planted.");
                return null;
            }

            // instantiate to world
            Plant plant = Instantiate(prefab, ground.point, Quaternion.identity);

            // set the gameobject's seed to the right one
            plant.name = seed.DisplayName;
            plant._seed = seed;
            plant._stage = 0;

            // render the seed sprite, and place it
            plant.ShowStage();
            plant.PlaceOn(ground.point.y);

            return plant;
        }

        // ================================= 2. grow =================================
        // when this plant get water, it grow
        public void Water()
        {
            if (_seed == null || IsGrown) return;

            _stage++;
            ShowStage();
        }

        // ================================= 3. harvest =================================
        // if the plant is grown, player press E on it:
        // the harvest is collected into player's backpack, and some of its seed drop on the ground
        public void Interact(Player player)
        {
            if (!IsGrown) return;

            ItemPickup.Spawn(_seed.Harvest, transform.position);
            DropSeeds();
            Destroy(gameObject);
        }

        private void DropSeeds()
        {
            int count = _seed.SeedsOnHarvest;
            Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
            float first = -(count - 1) * SeedDropSpread / 2f;

            for (int i = 0; i < count; i++)
            {
                ItemPickup.Spawn(_seed, origin + new Vector2(first + i * SeedDropSpread, 0f));
            }
        }

        // ================================= private =================================
        // render the seed sprite
        private void ShowStage()
        {
            if (_renderer != null) _renderer.sprite = _seed.StageSprite(_stage);
        }

        // sit the sprite's bottom edge on the ground
        private void PlaceOn(float groundY)
        {
            if (_renderer == null || _renderer.sprite == null) return;

            float bottomToPivot = transform.position.y - _renderer.bounds.min.y;
            transform.position = new Vector3(transform.position.x, groundY + bottomToPivot, transform.position.z);
        }

        void Awake()
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
        }
    }
}
