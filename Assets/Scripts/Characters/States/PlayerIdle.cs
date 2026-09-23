
namespace SideScroller.Characters.States
{
    /// <summary>
    /// Grounded and standing still. The resting state every other state falls back to.
    /// </summary>
    internal class PlayerIdle : PlayerState
    {
        public PlayerIdle(Player me, Transition transition) : base(me, transition) { }

        public override PlayerStateEnum StateType => PlayerStateEnum.Idle;

        public override void OnEnter()
        {
            _me.StopHorizontal();
            _me.PlayAnimation(PlayerStateEnum.Idle);
        }

        public override void OnUpdate()
        {
            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            // when player is on the ground, player can jump
            if (_transition.WantsJump()) { _me.ChangeState(PlayerStateEnum.Jump); return; }

            // when player is not on the ground by jumping, player is falling
            if (!_me.IsGrounded) { _me.ChangeState(PlayerStateEnum.Fall); return; }

            // when player want to walk, let player walk 
            if (_transition.WantsWalk()) { _me.ChangeState(PlayerStateEnum.Walk); return; }
        }
    }
}
