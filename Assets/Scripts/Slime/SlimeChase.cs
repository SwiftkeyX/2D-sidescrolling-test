
namespace SideScroller.Enemies.States
{
    internal class SlimeChase : SlimeState
    {
        public SlimeChase(Slime me, SlimeTransition transition) : base(me, transition) { }

        public override SlimeStateEnum StateType => SlimeStateEnum.Chase;

        public override void OnEnter()
        {
            _me.PlayAnimation(SlimeStateEnum.Chase);
        }

        public override void OnUpdate()
        {
            _me.FaceMoveDirection(_me.DirectionToTarget);
            _me.MoveHorizontal(_me.Facing);

            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            if (_transition.LostTarget()) { _me.ChangeState(SlimeStateEnum.Idle); return; }
        }
    }
}
