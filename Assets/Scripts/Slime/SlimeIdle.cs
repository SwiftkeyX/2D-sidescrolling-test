using UnityEngine;

namespace SideScroller.Enemies.States
{
    /// <summary>
    /// A short pause after doing something
    /// </summary>
    internal class SlimeIdle : SlimeState
    {
        private float _enteredAt;

        public SlimeIdle(Slime me, SlimeTransition transition) : base(me, transition) { }

        public override SlimeStateEnum StateType => SlimeStateEnum.Idle;

        public override void OnEnter()
        {
            _enteredAt = Time.time;

            _me.StopHorizontal();
            _me.PlayAnimation(SlimeStateEnum.Idle);
        }

        public override void OnUpdate()
        {
            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            // if slime see target, chase
            if (_transition.SeesTarget()) { _me.ChangeState(SlimeStateEnum.Chase); return; }

            // after a short pause, patrol
            if (Time.time - _enteredAt >= _me.IdleDuration) { _me.ChangeState(SlimeStateEnum.Patrol); return; }
        }
    }
}
