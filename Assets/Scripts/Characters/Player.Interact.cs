using SideScroller.Interactions;
using UnityEngine;

namespace SideScroller.Characters
{
    /// part of the Player: pressing E on things in the world.
    /// finds the closest IInteractable within reach and lets it decide what E means
    /// e.g. storage chest, crafting station, grown plant
    public partial class Player
    {
        // ======================================== interact ========================================
        // press E on the closest interactable thing within reach
        public void TryInteract()
        {
            Vector2 from = transform.position;
            IInteractable closest = null;
            float best = float.MaxValue;

            // chcek if my position reach any interactable
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(from, _interactReach))
            {
                IInteractable target = hit.GetComponentInParent<IInteractable>();
                if (target == null) continue;

                // measured to the collider's edge
                float distance = Vector2.Distance(from, hit.ClosestPoint(from));
                if (distance >= best) continue;

                best = distance;
                closest = target;
            }

            // interact with the cloest one
            closest?.Interact(this);
        }
    }
}
