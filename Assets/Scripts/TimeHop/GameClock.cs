using System;
using UnityEngine;

namespace SideScroller.TimeHop
{
    /// Control time of the day by advance time period. (Morning/Afternoon/Evening/Morning)
    public class GameClock : MonoBehaviour
    {
        // 5 minutes is one interval
        [SerializeField] private float _intervalSeconds = 10f;
        [SerializeField] private TimePeriodEnum _startPeriod = TimePeriodEnum.Morning;

        private float _periodStartedAt;
        public TimePeriodEnum Period { get; private set; }

        public event Action<TimePeriodEnum> PeriodChanged;

        void Awake()
        {
            Period = _startPeriod;
            _periodStartedAt = Time.time;
        }

        void Update()
        {
            float TimeInThePeriod = Time.time - _periodStartedAt;
            if (TimeInThePeriod < _intervalSeconds) return;

            Advance();
        }

        // Advance time period. (Morning/Afternoon/Evening/Morning)
        public void Advance()
        {
            Period = Period == TimePeriodEnum.Evening ? TimePeriodEnum.Morning : Period + 1;
            _periodStartedAt = Time.time;

            PeriodChanged?.Invoke(Period);
        }
    }
}
