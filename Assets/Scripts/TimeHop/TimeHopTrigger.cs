using SideScroller.Characters;
using UnityEngine;

namespace SideScroller.TimeHop
{
    /// Walk into it and time moves on. This is the spec's way of testing time hops
    /// without sitting through a whole interval.
    [RequireComponent(typeof(Collider2D))]
    public class TimeHopTrigger : MonoBehaviour
    {
        [SerializeField] private GameClock _clock;
        [SerializeField] private float _cooldown = 1f;

        private float _lastHopAt = float.NegativeInfinity;

        void Awake()
        {
            if (_clock == null) _clock = FindFirstObjectByType<GameClock>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (_clock == null) return;
            if (Time.time - _lastHopAt < _cooldown) return;

            // if not player, return
            if (other.GetComponentInParent<Player>() == null) return;

            _lastHopAt = Time.time;

            _clock.Advance();
        }

        void OnDrawGizmos()
        {
            Collider2D area = GetComponent<Collider2D>();
            if (area == null) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(area.bounds.center, area.bounds.size);
        }
    }
}
