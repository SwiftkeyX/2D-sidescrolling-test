using SideScroller.Characters;
using SideScroller.Enemies.States;
using UnityEngine;

namespace SideScroller.Enemies
{
    /// <summary>
    /// Same idea as Player: the only Monobehaviour for the slime, pure glue.
    /// The state machine decides what to do, this exposes what the slime can sense and do.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Slime : MonoBehaviour
    {
        // ======================================== Dependency ========================================
        // ===== classes =====
        private SlimeStateMachine _stateMachine;

        // ===== unity =====
        private Rigidbody2D _body;
        private SpriteRenderer _sprite;
        private Transform _target;

        // ======================================== Etc ========================================
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _detectRange = 5f;

        // ======================================== getter ========================================
        // ====== 1. tuning ======
        public float DetectRange => _detectRange;

        // ====== 2. state ======
        public void ChangeState(SlimeStateEnum next) => _stateMachine.ChangeState(next);
        public SlimeStateEnum StateType => _stateMachine.CurrentType;
        public SlimeStateEnum PreviousStateType => _stateMachine.PreviousType;

        // ====== 3. senses ======
        // -1 or 1, the way we are facing
        public float Facing { get; private set; } = 1f;

        public bool HasTarget => _target != null;
        public float DistanceToTarget => _target == null ? float.MaxValue : Vector2.Distance(transform.position, _target.position);
        public float DirectionToTarget => _target == null ? Facing : Mathf.Sign(_target.position.x - transform.position.x);

        // ======================================== unity ========================================
        void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _sprite = GetComponent<SpriteRenderer>();

            Player player = FindFirstObjectByType<Player>();
            if (player != null) _target = player.transform;

            _stateMachine = new SlimeStateMachine(this);
        }

        void Start()
        {
            _stateMachine.Start(SlimeStateEnum.Idle);
        }

        void Update()
        {
            _stateMachine.Tick();
        }

        // ======================================== state machine ========================================
        // ==== movement ====
        public void MoveHorizontal(float direction)
        {
            _body.linearVelocity = new Vector2(direction * _moveSpeed, _body.linearVelocity.y);
        }

        public void StopHorizontal()
        {
            _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
        }

        public void FaceMoveDirection(float direction)
        {
            if (Mathf.Approximately(direction, 0f)) return;

            Facing = Mathf.Sign(direction);

            if (_sprite != null) _sprite.flipX = Facing < 0f;
        }

        // ==== other ====
        // no Animator yet
        public void PlayAnimation(SlimeStateEnum state) { }

        // ======================================== editor ========================================
#if UNITY_EDITOR
        // draw circle radius indicate detection range
        void OnDrawGizmosSelected()
        {
            bool chasing = Application.isPlaying && StateType == SlimeStateEnum.Chase;

            UnityEditor.Handles.color = chasing ? Color.red : Color.yellow;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, _detectRange);
        }
#endif
    }
}
