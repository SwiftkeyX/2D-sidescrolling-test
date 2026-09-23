
namespace SideScroller.Enemies.States
{
    internal class SlimeTransition
    {
        // leaving costs more range than entering, so the slime does not flicker at the edge of sight
        private const float LoseSightMultiplier = 1.3f;

        private readonly Slime _me;

        public SlimeTransition(Slime me)
        {
            _me = me;
        }

        // ======================================== transition condition ========================================
        // player is in detection range
        public bool SeesTarget()
        {
            return _me.HasTarget && _me.DistanceToTarget <= _me.DetectRange;
        }

        // player is not in detection range
        public bool LostTarget()
        {
            return !_me.HasTarget || _me.DistanceToTarget > _me.DetectRange * LoseSightMultiplier;
        }
    }
}
