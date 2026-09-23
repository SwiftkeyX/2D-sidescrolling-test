
namespace SideScroller.Enemies.States
{
    internal abstract class SlimeState
    {
        protected readonly Slime _me;
        protected readonly SlimeTransition _transition;

        protected SlimeState(Slime me, SlimeTransition transition)
        {
            _me = me;
            _transition = transition;
        }

        public abstract SlimeStateEnum StateType { get; }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public abstract void OnUpdate();
        protected abstract void CheckSwitchState();
    }
}
