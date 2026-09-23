
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
            // when player is on the ground, player can jump
            if (_transition.WantsJump()) { _me.ChangeState(PlayerStateEnum.Jump); return; }

            // when player is not on the ground by jumping, player is falling
            if (!_me.IsGrounded) { _me.ChangeState(PlayerStateEnum.Fall); return; }
            
            // when player want to walk, let player walk 
            if (!_transition.WantsWalk()) { _me.ChangeState(PlayerStateEnum.Idle); return; }
        }
    }
}
