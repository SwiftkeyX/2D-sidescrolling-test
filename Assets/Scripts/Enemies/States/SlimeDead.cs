
namespace SideScroller.Enemies.States
{
    internal class SlimeDead : SlimeState
    {
        public SlimeDead(Slime me, SlimeTransition transition) : base(me, transition) { }

        public override SlimeStateEnum StateType => SlimeStateEnum.Dead;

        public override void OnEnter()
        {
            _me.StopHorizontal();
            _me.PlayAnimation(SlimeStateEnum.Dead);

            _me.SplitIntoSmallerSlime();
            _me.Despawn();
        }

        public override void OnUpdate() { }

        // there is no way out of this one
        protected override void CheckSwitchState() { }
    }
}
