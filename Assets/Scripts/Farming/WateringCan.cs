using SideScroller.Characters;
using SideScroller.Equipments;
using UnityEngine;

namespace SideScroller.Farming
{
    /// Left click with the watering can equipped: water the plant under the pointer, if it is within reach.
    public class WateringCan : Equipment
    {
        [SerializeField] private float _reach = 3f;         // how far from the player a plant can be watered
        [SerializeField] private float _aimRadius = 0.5f;   // how close to the plant the pointer has to be

        private Player _player;

        public override void Activate(Vector2 direction)
        {
            if (_player == null) _player = GetComponentInParent<Player>();
            if (_player == null) return;

            Vector2 target = _player.GetPointerWorldPosition();
            if (Vector2.Distance(_player.transform.position, target) > _reach) return;

            foreach (Collider2D hit in Physics2D.OverlapCircleAll(target, _aimRadius))
            {
                Plant plant = hit.GetComponent<Plant>();
                if (plant == null) continue;

                plant.Water();
                return;
            }
        }
    }
}
