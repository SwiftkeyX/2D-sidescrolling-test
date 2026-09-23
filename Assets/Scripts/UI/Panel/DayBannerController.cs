using SideScroller.TimeHop;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// The centred day banner. Shows on the Evening -> Morning turn, then fades out
    internal class DayBannerController : PanelController
    {
        private const string ShownClass = "banner--shown";

        [SerializeField] private GameClock _clock;
        [SerializeField] private float _holdSeconds = 1.5f;

        private VisualElement _banner;
        private Label _text;

        // =================================== Life cycle ===================================
        protected override void OnMounted(VisualElement panel)
        {
            _banner = panel.Q<VisualElement>("DayBanner");
            _text = panel.Q<Label>("DayBannerText");

            if (_clock == null) _clock = FindFirstObjectByType<GameClock>();
            if (_clock == null) return;

            _clock.DayChanged += Show;
        }

        void OnDisable()
        {
            if (_clock != null) _clock.DayChanged -= Show;
        }

        // =================================== private ===================================
        private void Show(WeekdayEnum weekday, int day)
        {
            if (_banner == null || _text == null) return;

            _text.text = $"{weekday.ToString().ToUpperInvariant()} DAY {day}";
            _banner.AddToClassList(ShownClass);

            Invoke(nameof(Hide), _holdSeconds);
        }

        private void Hide()
        {
            if (_banner != null) _banner.RemoveFromClassList(ShownClass);
        }
    }
}
