
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

        public SlimeState Current { get; private set; }

        public SlimeStateEnum CurrentType => Current == null ? SlimeStateEnum.Idle : Current.StateType;
        public SlimeStateEnum PreviousType { get; private set; }

        public SlimeStateMachine(Slime slime)
        {
            SlimeTransition transition = new SlimeTransition(slime);

            _idle = new SlimeIdle(slime, transition);
            _patrol = new SlimePatrol(slime, transition);
            _chase = new SlimeChase(slime, transition);
            _attack = new SlimeAttack(slime, transition);
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

            Current.OnUpdate();
        }

        private SlimeState GetState(SlimeStateEnum type)
        {
            switch (type)
            {
                case SlimeStateEnum.Idle: return _idle;
                case SlimeStateEnum.Patrol: return _patrol;
                case SlimeStateEnum.Chase: return _chase;
                case SlimeStateEnum.Attack: return _attack;
                default: return _idle;
            }
        }
    }
}
