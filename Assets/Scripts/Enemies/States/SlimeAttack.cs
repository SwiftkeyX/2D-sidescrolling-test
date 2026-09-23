
namespace SideScroller.Enemies.States
{
    // slime attack by jumping
    internal class SlimeAttack : SlimeState
    {
        private bool _leftGround;

        public SlimeAttack(Slime me, SlimeTransition transition) : base(me, transition) { }

        public override SlimeStateEnum StateType => SlimeStateEnum.Attack;

        public override void OnEnter()
        {
            _leftGround = false;

            _me.FaceMoveDirection(_me.DirectionToTarget);
            _me.PlayAnimation(SlimeStateEnum.Attack);

            _me.Jump(_me.Facing);
        }

        public override void OnUpdate()
        {
            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            // if slime isn't on the ground, return
            if (!_me.IsGrounded) { _leftGround = true; return; }
            if (!_leftGround) return;

            if (_transition.LostTarget()) { _me.ChangeState(SlimeStateEnum.Idle); return; }

            if (_transition.SeesTarget()) { _me.ChangeState(SlimeStateEnum.Chase); return; }
        }
    }
}
