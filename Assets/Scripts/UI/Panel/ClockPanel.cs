using SideScroller.TimeHop;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// The clock in the top-right of the overlay. listen to GameClock
    internal class ClockPanel : PanelController
    {
        [SerializeField] private GameClock _clock;

        private Label _period;

        // =================================== public ===================================
        public void ShowTime(TimePeriodEnum period)
        {
            if (_period != null) _period.text = period.ToString();
        }

        // =================================== Life cycle ===================================
        protected override void OnMounted(VisualElement panel)
        {
            _period = panel.Q<Label>("ClockPeriod");

            if (_clock == null) _clock = FindFirstObjectByType<GameClock>();
            if (_clock == null) return;

            _clock.PeriodChanged += ShowTime;
            ShowTime(_clock.Period);
        }

        void OnDisable()
        {
            if (_clock != null) _clock.PeriodChanged -= ShowTime;
        }
    }
}
