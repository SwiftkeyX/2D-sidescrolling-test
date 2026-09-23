using System;
using UnityEngine;

namespace SideScroller.TimeHop
{
    /// Control time of the day by advance time period. (Morning/Afternoon/Evening/Morning)
    /// advance time each day. (DAY 1/2/3/4/...) (MON/TUE/WED/etc...)
    public class GameClock : MonoBehaviour
    {
        // 5 minutes is one interval
        [SerializeField] private float _intervalSeconds = 10f;
        [SerializeField] private TimePeriodEnum _startPeriod = TimePeriodEnum.Morning;
        [SerializeField] private WeekdayEnum _startWeekday = WeekdayEnum.Monday;
        [SerializeField] private int _startDay = 1;

        private float _periodStartedAt;
        public TimePeriodEnum Period { get; private set; }
        public WeekdayEnum Weekday { get; private set; }
        public int Day { get; private set; }

        public event Action<TimePeriodEnum> PeriodChanged;
        public event Action<WeekdayEnum, int> DayChanged;

        void Awake()
        {
            Period = _startPeriod;
            Weekday = _startWeekday;
            Day = _startDay;

            _periodStartedAt = Time.time;
        }

        void Update()
        {
            float TimeInThePeriod = Time.time - _periodStartedAt;
            if (TimeInThePeriod < _intervalSeconds) return;

            Advance();
        }

        // Advance time period and day. 
        // time period = (Morning/Afternoon/Evening/Morning)
        // day = (MON/TUE/WED/etc...)
        public void Advance()
        {
            // after evening, new day come
            bool isNewDay = Period == TimePeriodEnum.Evening;

            Period = isNewDay ? TimePeriodEnum.Morning : Period + 1;
            _periodStartedAt = Time.time;
            
            // advance period
            PeriodChanged?.Invoke(Period);

            // advance day
            if (!isNewDay) return;
            Day++;
            Weekday = Weekday == WeekdayEnum.Sunday ? WeekdayEnum.Monday : Weekday + 1;

            DayChanged?.Invoke(Weekday, Day);
        }
    }
}
