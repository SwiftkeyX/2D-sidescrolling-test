
namespace SideScroller.Characters.States
{
    internal class PlayerDead : PlayerState
    {
        public PlayerDead(Player me, Transition transition) : base(me, transition) { }

        public override PlayerStateEnum StateType => PlayerStateEnum.Dead;

        public override void OnEnter()
        {
            _me.StopHorizontal();
            _me.PlayAnimation(PlayerStateEnum.Dead);
        }

        public override void OnUpdate() { }

        protected override void CheckSwitchState() { }
    }
}
