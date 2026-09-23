using MagicSchool.Combat.Heroes.States;
using UnityEngine;

namespace MagicSchool.Combat.Heroes
{
    /// Hero don't have any logic inside it BUT:
    /// 1) It's the ONLY Monobehavior for the Hero, so it's here so we could make hero interact with Unity.
    /// 2) it act like a glue, which mean itself don't contain any real logic.
    /// </summary>
    public class Player : MonoBehaviour
    {
        // ======================================== Dependency ========================================
        private PlayerStateMachine _stateMachine;

        // ======================================== Etc ========================================
        [SerializeField] private float _moveSpeed = 1f;

        // ======================================== getter ======================================== 
        public void ChangeState(PlayerStateEnum next) => _stateMachine.ChangeState(next);
        public PlayerStateEnum PreviousStateType => _stateMachine.PreviousType;
        public PlayerStateEnum StateType => _stateMachine.CurrentType;

        // ======================================== life cycle ========================================
        void Start()
        {
            _stateMachine.Start(PlayerStateEnum.Idle);
        }

        void Update()
        {
            _stateMachine.Tick();
        }
    }
}
