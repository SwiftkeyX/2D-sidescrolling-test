using SideScroller.Inventories;
using UnityEngine;

namespace SideScroller.Farming
{
    /// <summary>
    /// Data for one seed. A seed can be place on the ground.
    /// Once plated it could also grow into different shape. 
    /// </summary>
    [CreateAssetMenu(fileName = "Seed", menuName = "SideScroller/Seed")]
    public class SeedSO : ItemSO
    {
        // all the sprite for this seed evolution
        // e.g. first stage = Carrot Seed, last stage = fully grown Carrot
        [SerializeField] private Sprite[] _growthStages;

        // this is what the player gets when harvesting the fully grown plant
        // e.g. Carrot
        [SerializeField] private ItemSO _harvest;

        // when harvest, drop x amount of seed
        [SerializeField, Min(0)] private int _seedsOnHarvest = 3;

        public int StageCount => _growthStages == null ? 0 : _growthStages.Length;
        public int GrownStage => StageCount - 1;
        public ItemSO Harvest => _harvest;
        public int SeedsOnHarvest => _seedsOnHarvest;

        // the sprite for one growth stage. null if there is no such stage
        public Sprite StageSprite(int stage)
        {
            if (_growthStages == null || stage < 0 || stage >= _growthStages.Length) return null;

            return _growthStages[stage];
        }

        // SeedSO are always categorized as Seed
        public override ItemCategoryEnum Category => ItemCategoryEnum.Seed;
    }
}
