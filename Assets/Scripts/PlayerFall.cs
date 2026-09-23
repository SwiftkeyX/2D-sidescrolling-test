
namespace SideScroller.Characters.States
{
    /// <summary>
    /// Fall is when player in the air and moving downward
    /// conterpart to Jump
    /// </summary>
    internal class PlayerFall : PlayerState
    {
        public PlayerFall(Player me, Transition transition) : base(me, transition) { }

        public override PlayerStateEnum StateType => PlayerStateEnum.Fall;

        public override void OnEnter()
        {
            _me.PlayAnimation(PlayerStateEnum.Fall);
        }

        public override void OnUpdate()
        {
            // player can move during fall state
            _me.MoveHorizontal(_me.MoveInput);
            _me.FaceMoveDirection(_me.MoveInput);

            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            if (_me.IsGrounded) { _me.ChangeState(PlayerStateEnum.Idle); return; }
        }
    }
}
