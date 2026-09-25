using SideScroller.Characters;
using SideScroller.Equipments;
using UnityEngine;

namespace SideScroller.Woodcutting
{
    /// Left click with the axe equipped: chop the tree under the pointer, if it is within reach.
    public class Axe : Equipment
    {
        [SerializeField] private float _reach = 2f;         // how far from the player the tree can be
        [SerializeField] private float _aimRadius = 0.5f;   // how close to the tree the pointer has to be

        private Player _player;

        public override void Activate(Vector2 direction)
        {
            if (_player == null) _player = GetComponentInParent<Player>();
            if (_player == null) return;

            Vector2 target = _player.GetPointerWorldPosition();
            Vector2 from = _player.transform.position;

            foreach (Collider2D hit in Physics2D.OverlapCircleAll(target, _aimRadius))
            {
                ChoppableTree tree = hit.GetComponentInParent<ChoppableTree>();
                if (tree == null) continue;

                // reach is measured to the tree's edge
                if (Vector2.Distance(from, hit.ClosestPoint(from)) > _reach) continue;

                tree.Chop();
                return;
            }
        }
    }
}
