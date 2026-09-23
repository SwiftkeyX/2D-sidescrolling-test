
namespace MagicSchool.Combat.Heroes.States
{

    internal abstract class PlayerState
    {
        protected readonly Player _me;
        protected readonly Transition _transition;

        protected PlayerState(Player me, Transition transition)
        {
            _me = me;
            _transition = transition;
        }

        public abstract PlayerStateEnum StateType { get; }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public abstract void OnUpdate();
        protected abstract void CheckSwitchState();

    }
}
