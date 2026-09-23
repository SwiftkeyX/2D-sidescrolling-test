
using UnityEngine;

namespace SideScroller.Characters.States
{
    internal class PlayerDead : PlayerState
    {
        private float _enteredAt;

        public PlayerDead(Player me, Transition transition) : base(me, transition) { }

        public override PlayerStateEnum StateType => PlayerStateEnum.Dead;

        public override void OnEnter()
        {
            _enteredAt = Time.time;

            _me.StopHorizontal();
            _me.PlayAnimation(PlayerStateEnum.Dead);
        }

        public override void OnUpdate()
        {
            CheckSwitchState();
        }

        protected override void CheckSwitchState()
        {
            // after enter dead state, short pause here before respawn
            if (Time.time - _enteredAt < _me.RespawnDelay) return;
            _me.Respawn();

            // after die, default to idle state
            _me.ChangeState(PlayerStateEnum.Idle);
        }
    }
}
