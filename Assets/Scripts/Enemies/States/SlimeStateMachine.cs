using SideScroller.Combat;

namespace SideScroller.Enemies.States
{
    /// <summary>
    /// Same shape as the player's machine, driven by what the slime senses instead of input.
    /// </summary>
    internal class SlimeStateMachine
    {
        private readonly SlimeIdle _idle;
        private readonly SlimePatrol _patrol;
        private readonly SlimeChase _chase;
        private readonly SlimeAttack _attack;
        private readonly SlimeDead _dead;

        private readonly Stat _stat;
        private bool _died;

        public SlimeState Current { get; private set; }

        public SlimeStateEnum CurrentType => Current == null ? SlimeStateEnum.Idle : Current.StateType;
        public SlimeStateEnum PreviousType { get; private set; }

        public SlimeStateMachine(Slime slime, Stat stat)
        {
            SlimeTransition transition = new SlimeTransition(slime);

            _idle = new SlimeIdle(slime, transition);
            _patrol = new SlimePatrol(slime, transition);
            _chase = new SlimeChase(slime, transition);
            _attack = new SlimeAttack(slime, transition);
            _dead = new SlimeDead(slime, transition);

            _stat = stat;
            if (_stat != null) _stat.Died += OnDied;
        }

        public void Start(SlimeStateEnum initial)
        {
            Current = GetState(initial);
            Current.OnEnter();
        }

        public void ChangeState(SlimeStateEnum next)
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
            if (TryResolveInterrupt(out SlimeStateEnum forced))
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
        private bool TryResolveInterrupt(out SlimeStateEnum forced)
        {
            forced = default;

            // guard
            if (CurrentType == SlimeStateEnum.Dead) return false;

            if (_died)
            {
                _died = false;
                forced = SlimeStateEnum.Dead;
                return true;
            }

            // add more interrupt
            // ...

            return false;
        }

        private SlimeState GetState(SlimeStateEnum type)
        {
            switch (type)
            {
                case SlimeStateEnum.Idle: return _idle;
                case SlimeStateEnum.Patrol: return _patrol;
                case SlimeStateEnum.Chase: return _chase;
                case SlimeStateEnum.Attack: return _attack;
                case SlimeStateEnum.Dead: return _dead;
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
