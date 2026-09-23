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
        private Collider2D _collider;
        private SpriteRenderer _sprite;
        private Transform _target;
        private ContactFilter2D _solidFilter;
        private readonly RaycastHit2D[] _probeHits = new RaycastHit2D[1];

        // ======================================== Etc ========================================
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _detectRange = 5f;
        [SerializeField] private float _idleDuration = 0.6f;

        private const float WallCheckDistance = 0.08f;

        // ======================================== getter ========================================
        // ====== 1. tuning ======
        public float DetectRange => _detectRange;
        public float IdleDuration => _idleDuration;

        // ====== 2. state ======
        public void ChangeState(SlimeStateEnum next) => _stateMachine.ChangeState(next);
        public SlimeStateEnum StateType => _stateMachine.CurrentType;
        public SlimeStateEnum PreviousStateType => _stateMachine.PreviousType;

        // ====== 3. senses ======
        public bool HasTarget => _target != null;
        public float DistanceToTarget => _target == null ? float.MaxValue : Vector2.Distance(transform.position, _target.position);
        public float DirectionToTarget => _target == null ? Facing : Mathf.Sign(_target.position.x - transform.position.x);
        
        // -1 or 1, the way we are facing
        public float Facing { get; private set; } = 1f;

        // Is there a wall ahead?
        public bool HasWallAhead => _collider.Cast(new Vector2(Facing, 0f), _solidFilter, _probeHits, WallCheckDistance) > 0;

        // ======================================== life cycle ========================================
        void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _sprite = GetComponent<SpriteRenderer>();

            _solidFilter = new ContactFilter2D();
            _solidFilter.useTriggers = false;

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

        public void TurnAround() => FaceMoveDirection(-Facing);

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
        // draw circle radius indicate detection range
        void OnDrawGizmos()
        {
            bool chasing = Application.isPlaying && _stateMachine != null && StateType == SlimeStateEnum.Chase;

            Gizmos.color = chasing ? Color.red : Color.yellow;
            DrawCircle(transform.position, _detectRange);
        }

        private static void DrawCircle(Vector3 centre, float radius, int segments = 48)
        {
            Vector3 previous = centre + new Vector3(radius, 0f, 0f);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i / (float)segments * Mathf.PI * 2f;
                Vector3 next = centre + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);

                Gizmos.DrawLine(previous, next);
                previous = next;
            }
        }

    }
}
