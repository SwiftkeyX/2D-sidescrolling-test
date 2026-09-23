
namespace SideScroller.Characters.States
{
    /// <summary>
    /// PlayerStateMachine is state machine that control player's behaviour.
    /// It's vanilla state machine, nothing special.
    /// </summary>
    internal class PlayerStateMachine
    {
        private readonly PlayerIdle _idle;
        private readonly PlayerWalk _walk;
        private readonly PlayerJump _jump;
        private readonly PlayerFall _fall;

        public PlayerState Current { get; private set; }

        public PlayerStateEnum CurrentType => Current == null ? PlayerStateEnum.Idle : Current.StateType;
        public PlayerStateEnum PreviousType { get; private set; }

        public PlayerStateMachine(Player player)
        {
            Transition transition = new Transition(player);

            _idle = new PlayerIdle(player, transition);
            _walk = new PlayerWalk(player, transition);
            _jump = new PlayerJump(player, transition);
            _fall = new PlayerFall(player, transition);
        }

        public void Start(PlayerStateEnum initial)
        {
            Current = GetState(initial);
            Current.OnEnter();
        }

        public void ChangeState(PlayerStateEnum next)
        {
            if (Current != null && next == CurrentType) return;

            PreviousType = CurrentType;

            Current?.OnExit();
            Current = GetState(next);
            Current.OnEnter();
        }

        public void Tick()
        {
            if (Current == null) return;

            // global interrupts (dead, stun) get resolved here before the state updates,
            // once there are states that need them

            Current.OnUpdate();
        }

        private PlayerState GetState(PlayerStateEnum type)
        {
            switch (type)
            {
                case PlayerStateEnum.Idle: return _idle;
                case PlayerStateEnum.Walk: return _walk;
                case PlayerStateEnum.Jump: return _jump;
                case PlayerStateEnum.Fall: return _fall;
                default: return _idle;
            }
        }
    }
}
