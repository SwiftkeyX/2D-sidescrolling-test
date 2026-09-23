
namespace MagicSchool.Combat.Heroes.States
{

    internal abstract class PlayerState
    {
        // protected readonly Player _me;
        protected readonly Transition _transition;

        protected PlayerState(Transition transition)
        {
            _transition = transition;
        }

        public abstract PlayerStateEnum StateType { get; }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public abstract void OnUpdate();
        protected abstract void CheckSwitchState();

    }
}
