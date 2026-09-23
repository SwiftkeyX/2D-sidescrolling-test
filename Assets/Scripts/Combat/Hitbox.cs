using UnityEngine;

namespace SideScroller.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private int _damage = 3;
        [SerializeField] private TeamEnum _targetTeam = TeamEnum.Player;
        [SerializeField] private float _hitCooldown = 0.5f;

        private float _lastHitAt = float.NegativeInfinity;

        // OnEnter hitbox, apply damage
        void OnTriggerEnter2D(Collider2D other) => TryDamage(other);

        // OnStay hitbox, apply damage but guard with _hitcooldown
        void OnTriggerStay2D(Collider2D other) => TryDamage(other);

        private void TryDamage(Collider2D other)
        {
            if (Time.time - _lastHitAt < _hitCooldown) return;

            // the collider may be a child, health lives further up
            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null) return;

            // damage only applies to the other team
            if (target.Team != _targetTeam) return;

            _lastHitAt = Time.time;
            target.TakeDamage(_damage);
        }  
    }
}
