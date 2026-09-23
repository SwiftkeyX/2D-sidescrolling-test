
namespace SideScroller.Characters.States
{
    internal class Transition
    {
        private readonly Player _me;

        public Transition(Player me)
        {
            _me = me;
        }

        // ======================================== transition condition ========================================
        // when there's input, allow player to walk
        public bool WantsWalk()
        {
            if (_me.MoveInput != 0f) return true;

            return false;
        }

        // whne player is on the ground, allow player to jump 
        public bool WantsJump()
        {
            return _me.JumpPressed && _me.IsGrounded;
        }

        // when player is falling.
        public bool IsFalling()
        {
            return _me.VerticalVelocity <= 0f;
        }
    }
}
