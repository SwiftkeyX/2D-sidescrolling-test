
namespace SideScroller.Enemies.States
{
    /// Walks forward until the wall gets in the way, then turns around.
    internal class SlimePatrol : SlimeState
    {
        public SlimePatrol(Slime me, SlimeTransition transition) : base(me, transition) { }

        public override SlimeStateEnum StateType => SlimeStateEnum.Patrol;

        public override void OnEnter()
        {
            _me.PlayAnimation(SlimeStateEnum.Patrol);
        }

        public override void OnUpdate()
        {
            // when there's something blocked ahead, turn around
            if (_transition.BlockedAhead()) _me.TurnAround();

            // move toward facing direction
            _me.MoveHorizontal(_me.Facing);

            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            // if slime see target, chase
            if (_transition.SeesTarget()) { _me.ChangeState(SlimeStateEnum.Chase); return; }
        }
    }
}
