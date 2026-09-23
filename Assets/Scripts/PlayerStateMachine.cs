
namespace MagicSchool.Combat.Heroes.States
{
    /// <summary>
    /// PlayerStateMachine is state machine that control hero's behaviour.
    /// It's vanilla state machine, nothing special.
    /// </summary>
    internal class PlayerStateMachine
    {
        private readonly Player _me;
        private readonly PlayerIdle _idle;
        private readonly PlayerWalk _walk;
        private readonly PlayerAttack _attack;
        private readonly PlayerDead _dead;

        public PlayerState Current { get; private set; }

        public PlayerStateEnum CurrentType => Current == null ? PlayerStateEnum.Idle : Current.StateType;
        public PlayerStateEnum PreviousType { get; private set; }

        public PlayerStateMachine(Player hero)
        {
            _me = hero;
            Transition transition = new Transition(hero);

            _idle = new PlayerIdle(hero, transition);
            _walk = new PlayerWalk(hero, transition);
            _attack = new PlayerAttack(hero, transition);
            _dead = new PlayerDead(hero, transition);
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

            // interrupt state e.g. stun, dead
            if (TryResolveInterrupt(out PlayerStateEnum forced))
            {
                ChangeState(forced);

                // return early, so we don't update in the same frame
                return;
            }

            // update state
            Current.OnUpdate();
        }

        /// <summary>
        /// Global state transition.  
        /// Some of the transition are redundant in each state, make it a global transition by move it here.
        /// </summary>
        private bool TryResolveInterrupt(out PlayerStateEnum forced)
        {
            forced = default;

            if (CurrentType == PlayerStateEnum.Dead) return false;

            if (_me.CurrentHP <= 0)
            {
                forced = PlayerStateEnum.Dead;
                return true;
            }

            // FLAGGING: the stun duration should be addition to previous exist stun running 
            bool notStun = CurrentType != PlayerStateEnum.Stunned;
            if (_me.IsStunned && notStun)
            {
                forced = PlayerStateEnum.Stunned;
                return true;
            }

            // FLAGGING: Being stun while casting skill is useless for the stun user, since skill effect is already fire.
            // if mana is full, trigger OnCast skill
            bool success = _me.TriggerActiveSkill(_me.IsManaCapped());
            if (success)
            {
                forced = PlayerStateEnum.Cast;
                return true;
            }

            return false;
        }

        private PlayerState GetState(PlayerStateEnum type)
        {
            switch (type)
            {
                case PlayerStateEnum.Idle: return _idle;
                case PlayerStateEnum.Walk: return _walk;
                case PlayerStateEnum.Attack: return _attack;
                case PlayerStateEnum.Dead: return _dead;
                default: return null;
            }
        }
    }
}
