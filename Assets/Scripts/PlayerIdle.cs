
namespace MagicSchool.Combat.Heroes.States
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
            if (_transition.WantsWalk()) { _me.ChangeState(PlayerStateEnum.Walk); return; }
        }
    }
}
