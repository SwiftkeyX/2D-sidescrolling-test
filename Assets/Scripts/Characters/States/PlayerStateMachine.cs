using SideScroller.Combat;

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
        private readonly PlayerDead _dead;

        private readonly Stat _stat;
        private bool _died;

        public PlayerState Current { get; private set; }

        public PlayerStateEnum CurrentType => Current == null ? PlayerStateEnum.Idle : Current.StateType;
        public PlayerStateEnum PreviousType { get; private set; }

        public PlayerStateMachine(Player player, Stat stat)
        {
            Transition transition = new Transition(player);

            _idle = new PlayerIdle(player, transition);
            _walk = new PlayerWalk(player, transition);
            _jump = new PlayerJump(player, transition);
            _fall = new PlayerFall(player, transition);
            _dead = new PlayerDead(player, transition);

            _stat = stat;
            if (_stat != null) _stat.Died += OnDied;
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

            // interrupt state e.g. dead
            if (TryResolveInterrupt(out PlayerStateEnum forced))
            {
                ChangeState(forced);

                // return early, so we don't update in the same frame
                return;
            }

            Current.OnUpdate();
        }

        /// <summary>
        /// Global state transition.  
        /// Some of the transition are redundant in each state, make it a global transition by move it here.
        /// </summary>
        private bool TryResolveInterrupt(out PlayerStateEnum forced)
        {
            forced = default;

            // guard
            if (CurrentType == PlayerStateEnum.Dead) return false;

            if (_died)
            {
                _died = false;
                forced = PlayerStateEnum.Dead;
                return true;
            }

            // add more interrupt
            // ...

            return false;
        }

        private PlayerState GetState(PlayerStateEnum type)
        {
            switch (type)
            {
                case PlayerStateEnum.Idle: return _idle;
                case PlayerStateEnum.Walk: return _walk;
                case PlayerStateEnum.Jump: return _jump;
                case PlayerStateEnum.Fall: return _fall;
                case PlayerStateEnum.Dead: return _dead;
                default: return _idle;
            }
        }

        // ========================= event handler =========================
        private void OnDied() => _died = true;

        // ========================= other =========================
        // destroy helper for non-monobehaviour class
        public void Detach()
        {
            if (_stat != null) _stat.Died -= OnDied;
        }
    }
}
