using System;
using UnityEngine;

namespace SideScroller.Combat
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 30;
        [SerializeField] private TeamEnum _team = TeamEnum.Player;

        public TeamEnum Team => _team;
        public int Max => _maxHealth;
        public int Current { get; private set; }
        public bool IsAlive => Current > 0;

        public event Action Died;

        void Awake()
        {
            Current = _maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;
            if (!IsAlive) return;

            Current = Mathf.Max(0, Current - amount);

            if (Current == 0) Died?.Invoke();
        }
    }
}
