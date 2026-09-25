using SideScroller.Characters.States;
using UnityEngine;

namespace SideScroller.Characters
{
    /// part of the Player: what the states (Idle, Walk, Jump, Fall, Dead) call on the player.
    /// 1) movement: walk, stop, jump, face left/right
    /// 2) ground check: is the player standing on the Ground layer?
    /// 3) dying: respawn at the checkpoint
    /// 4) animation (none yet)
    public partial class Player
    {
        private const float GroundCheckDistance = 0.05f;    // distance used by the cast to check the ground

        private ContactFilter2D _groundFilter;
        private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[1];

        // ======================================== ground check ========================================
        public bool IsGrounded => _collider.Cast(Vector2.down, _groundFilter, _groundHits, GroundCheckDistance) > 0;


        // ======================================== movement ========================================
        public float VerticalVelocity => _body.linearVelocity.y;

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
            if (_playerSprite == null) return;
            if (Mathf.Approximately(input, 0f)) return;

            _playerSprite.flipX = input < 0f;
        }


        // ======================================== dying ========================================
        public float RespawnDelay => _respawnDelay;

        // Respawn player at the checkpoint
        public void Respawn()
        {
            if (_checkpoint != null) transform.position = _checkpoint.position;

            if (_stat != null) _stat.ResetHealth();
        }


        // ======================================== animator ========================================
        // no Animator yet
        public void PlayAnimation(PlayerStateEnum state) { }


        // ======================================== init ========================================
        // init ground check 
        // using collider's cast function
        private void InitGroundCheck()
        {
            _groundFilter = new ContactFilter2D();
            _groundFilter.useTriggers = false;
            
            // only the Ground layer counts as ground
            // so the player can't jump off trees, slimes, etc.
            _groundFilter.SetLayerMask(LayerMask.GetMask("Ground"));
        }
    }
}
