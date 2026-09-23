using MagicSchool.Combat.Heroes.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MagicSchool.Combat.Heroes
{
    /// <summary>
    /// Player don't have any logic inside it BUT:
    /// 1) It's the ONLY Monobehavior for the Player, so it's here so we could make player interact with Unity.
    /// 2) it act like a glue, which mean itself don't contain any real logic.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        // ======================================== Dependency ========================================
        private PlayerStateMachine _stateMachine;
        private Rigidbody2D _body;
        private SpriteRenderer _sprite;

        // ======================================== Etc ========================================
        [SerializeField] private float _moveSpeed = 5f;

        // ======================================== getter ========================================
        // ====== 1. input ======
        public float MoveInput { get; private set; }

        // ====== 2. state ======
        public void ChangeState(PlayerStateEnum next) => _stateMachine.ChangeState(next);
        public PlayerStateEnum PreviousStateType => _stateMachine.PreviousType;
        public PlayerStateEnum StateType => _stateMachine.CurrentType;

        // ======================================== unity ========================================
        void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _stateMachine = new PlayerStateMachine(this);
        }

        void Start()
        {
            _stateMachine.Start(PlayerStateEnum.Idle);
        }

        void Update()
        {
            ReadInput();
            _stateMachine.Tick();
        }

        // ======================================== state machine ========================================
        public void MoveHorizontal(float input)
        {
            _body.linearVelocity = new Vector2(input * _moveSpeed, _body.linearVelocity.y);
        }

        public void StopHorizontal()
        {
            _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
        }

        public void FaceMoveDirection(float input)
        {
            if (_sprite == null) return;
            if (Mathf.Approximately(input, 0f)) return;

            _sprite.flipX = input < 0f;
        }

        // no Animator yet
        public void PlayAnimation(PlayerStateEnum state) { }

        // ======================================== input ========================================
        private void ReadInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) { MoveInput = 0f; return; }

            float input = 0f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input += 1f;

            MoveInput = input;
        }
    }
}
