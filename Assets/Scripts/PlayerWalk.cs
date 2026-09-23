
namespace SideScroller.Characters.States
{
    /// <summary>
    /// Grounded and moving horizontally. Movement is driven every frame from the raw input axis.
    /// </summary>
    internal class PlayerWalk : PlayerState
    {
        public PlayerWalk(Player me, Transition transition) : base(me, transition) { }

        public override PlayerStateEnum StateType => PlayerStateEnum.Walk;

        public override void OnEnter()
        {
            _me.PlayAnimation(PlayerStateEnum.Walk);
        }

        public override void OnUpdate()
        {
            _me.MoveHorizontal(_me.MoveInput);
            _me.FaceMoveDirection(_me.MoveInput);

            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            if (!_transition.WantsWalk()) { _me.ChangeState(PlayerStateEnum.Idle); return; }
        }
    }
}
