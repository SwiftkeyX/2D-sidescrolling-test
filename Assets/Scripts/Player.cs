using SideScroller.Characters.States;
using SideScroller.Input;
using UnityEngine;

namespace SideScroller.Characters
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
        private Collider2D _collider;
        private ContactFilter2D _groundFilter;
        private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[1];

        // ======================================== Etc ========================================
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 9f;
        private const float GroundCheckDistance = 0.05f;

        // ======================================== getter ========================================
        // ====== 1. input ======
        public float MoveInput => PlayerInputSystem.MoveAxis;
        public bool JumpPressed => PlayerInputSystem.JumpPressedThisFrame;

        // ====== 2. state ======
        public void ChangeState(PlayerStateEnum next) => _stateMachine.ChangeState(next);
        public PlayerStateEnum PreviousStateType => _stateMachine.PreviousType;
        public PlayerStateEnum StateType => _stateMachine.CurrentType;
        
        // ====== 3. jump state ======
        // Cast collider below to check ground state
        public bool IsGrounded => _collider.Cast(Vector2.down, _groundFilter, _groundHits, GroundCheckDistance) > 0;
        public float VerticalVelocity => _body.linearVelocity.y;

        // ======================================== unity ========================================
        void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();

            _groundFilter = new ContactFilter2D();
            _groundFilter.useTriggers = false;

            _stateMachine = new PlayerStateMachine(this);
        }

        void Start()
        {
            _stateMachine.Start(PlayerStateEnum.Idle);
        }

        void Update()
        {
            _stateMachine.Tick();
        }

        // ======================================== state machine ========================================
        // ==== movement ====
        public void MoveHorizontal(float input)
        {
            _body.linearVelocity = new Vector2(input * _moveSpeed, _body.linearVelocity.y);
        }

        public void StopHorizontal()
        {
            _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
        }

        public void ApplyJumpForce()
        {
            _body.linearVelocity = new Vector2(_body.linearVelocity.x, _jumpForce);
        }

        public void FaceMoveDirection(float input)
        {
            if (_sprite == null) return;
            if (Mathf.Approximately(input, 0f)) return;

            _sprite.flipX = input < 0f;
        }

        // ==== other ====
        // no Animator yet
        public void PlayAnimation(PlayerStateEnum state) { }
    }
}
