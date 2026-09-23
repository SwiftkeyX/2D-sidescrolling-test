using SideScroller.Characters;
using SideScroller.Combat;
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
        private Stat _stat;

        // ===== unity =====
        private Rigidbody2D _body;
        private Collider2D _collider;
        private SpriteRenderer _sprite;
        private Transform _target;
        private ContactFilter2D _solidFilter;
        private readonly RaycastHit2D[] _probeHits = new RaycastHit2D[1];

        // ======================================== tune data ========================================
        // slime move speed
        [SerializeField] private float _moveSpeed = 2f;

        // slime detection range
        [SerializeField] private float _detectRange = 5f;

        // short pause for idle state
        [SerializeField] private float _idleDuration = 0.6f;

        // attack data
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private float _attackCooldown = 1.5f;
        private float _lastAttackAt = float.NegativeInfinity;

        // jump data
        [SerializeField] private float _jumpUpForce = 5f;
        [SerializeField] private float _jumpForwardForce = 1.8f;

        // dead data
        // when dead this slime, split into smaller slime.
        [SerializeField] private Slime _splitIntoChild; 
        [SerializeField] private int _splitCount = 3;   
        [SerializeField] private float _splitSpread = 0.35f;

        // check wall & ground distance 
        private const float WallCheckDistance = 0.08f;
        private const float GroundCheckDistance = 0.05f;

        // ======================================== getter ========================================
        // ====== 1. tuning ======
        public float DetectRange => _detectRange;
        public float IdleDuration => _idleDuration;
        public float AttackRange => _attackRange;
        public bool AttackReady => Time.time - _lastAttackAt >= _attackCooldown;

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
        public bool IsGrounded => _collider.Cast(Vector2.down, _solidFilter, _probeHits, GroundCheckDistance) > 0;

        // ======================================== life cycle ========================================
        void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _stat = GetComponent<Stat>();

            _solidFilter = new ContactFilter2D();
            _solidFilter.useTriggers = false;

            Player player = FindFirstObjectByType<Player>();
            if (player != null) _target = player.transform;

            _stateMachine = new SlimeStateMachine(this, _stat);
        }

        void OnDestroy()
        {
            // destroy non-unity class
            _stateMachine?.Detach();
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

        public void Jump(float direction)
        {
            _lastAttackAt = Time.time;
            _body.linearVelocity = new Vector2(direction * _jumpForwardForce, _jumpUpForce);
        }

        public void TurnAround() => FaceMoveDirection(-Facing);

        public void FaceMoveDirection(float direction)
        {
            if (Mathf.Approximately(direction, 0f)) return;

            Facing = Mathf.Sign(direction);

            if (_sprite != null) _sprite.flipX = Facing < 0f;
        }

        // ==== dying ====
        public void Despawn() => Destroy(gameObject);

        // when this slime die, split into smaller slime
        public void SplitIntoSmallerSlime()
        {
            if (_splitIntoChild == null) return;
            if (_splitCount <= 0) return;

            // spread smaller slime along x axis
            float step = _splitCount > 1 ? _splitSpread : 0f;
            float startPos = -step * (_splitCount - 1) * 0.5f;

            // spawn slime
            for (int i = 0; i < _splitCount; i++)
            {
                Vector3 at = transform.position + new Vector3(startPos + step * i, 0f, 0f);
                Instantiate(_splitIntoChild, at, transform.rotation);
            }
        }

        // ==== other ====
        // no Animator yet
        public void PlayAnimation(SlimeStateEnum state) { }

        // ======================================== editor ========================================
        void OnDrawGizmos()
        {
            // draw circle radius indicate detection range
            bool chasing = Application.isPlaying && _stateMachine != null && StateType == SlimeStateEnum.Chase;
            Gizmos.color = chasing ? Color.red : Color.yellow;
            DrawCircle(transform.position, _detectRange);

            // draw circle radius indicate attack range
            Gizmos.color = Color.magenta;
            DrawCircle(transform.position, _attackRange);
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
