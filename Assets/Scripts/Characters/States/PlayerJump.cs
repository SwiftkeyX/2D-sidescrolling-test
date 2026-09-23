
namespace SideScroller.Characters.States
{
    /// <summary>
    /// Jump is when player in the air and moving upward
    /// </summary>
    internal class PlayerJump : PlayerState
    {
        public PlayerJump(Player me, Transition transition) : base(me, transition) { }

        public override PlayerStateEnum StateType => PlayerStateEnum.Jump;

        public override void OnEnter()
        {
            _me.PlayAnimation(PlayerStateEnum.Jump);
            _me.ApplyJumpForce();
        }

        public override void OnUpdate()
        {
            // player can move during jump
            _me.MoveHorizontal(_me.MoveInput);
            _me.FaceMoveDirection(_me.MoveInput);

            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            if (_transition.IsFalling()) { _me.ChangeState(PlayerStateEnum.Fall); return; }
        }
    }
}
