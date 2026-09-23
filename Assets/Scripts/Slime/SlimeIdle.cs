
namespace SideScroller.Enemies.States
{
    /// <summary>
    /// Sits still. Where the slime waits until the player turns up.
    /// </summary>
    internal class SlimeIdle : SlimeState
    {
        public SlimeIdle(Slime me, SlimeTransition transition) : base(me, transition) { }

        public override SlimeStateEnum StateType => SlimeStateEnum.Idle;

        public override void OnEnter()
        {
            _me.StopHorizontal();
            _me.PlayAnimation(SlimeStateEnum.Idle);
        }

        public override void OnUpdate()
        {
            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            if (_transition.SeesTarget()) { _me.ChangeState(SlimeStateEnum.Chase); return; }
        }
    }
}
