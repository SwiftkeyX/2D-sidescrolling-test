using System;
using UnityEngine;

namespace SideScroller.Combat
{
    public class Stat : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 30;
        [SerializeField] private TeamEnum _team = TeamEnum.Player;

        public TeamEnum Team => _team;
        public int Max => _maxHealth;
        public int Current { get; private set; }
        public bool IsAlive => Current > 0;

        public event Action Died;
        public event Action<int, int> OnHealthChanged;

        void Awake()
        {
            Current = _maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0) return;
            if (!IsAlive) return;

            Current = Mathf.Max(0, Current - amount);

            OnHealthChanged?.Invoke(Current, _maxHealth);

            if (Current == 0) Died?.Invoke();
        }

        public void ResetHealth()
        {
            Current = _maxHealth;
            OnHealthChanged?.Invoke(Current, _maxHealth);
        }
    }
}
