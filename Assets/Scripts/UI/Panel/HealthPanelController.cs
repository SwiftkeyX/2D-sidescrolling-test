using SideScroller.Combat;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    /// The player's health bar, top-left of the overlay.
    internal class HealthPanelController : PanelController
    {
        [SerializeField] private Health _health;

        private VisualElement _fill;
        private Label _value;

        // =================================== Life cycle ===================================
        protected override void OnMounted(VisualElement panel)
        {
            _fill = panel.Q<VisualElement>("HealthFill");
            _value = panel.Q<Label>("HealthValue");

            if (_health == null) _health = FindPlayerHealth();
            if (_health == null) return;

            _health.OnHealthChanged += Repaint;

            // update the health panel when the health is changed.
            Repaint(_health.Current, _health.Max);
        }

        void OnDisable()
        {
            if (_health != null) _health.OnHealthChanged -= Repaint;
        }

        // =================================== private ===================================
        private void Repaint(int current, int max)
        {
            float percent = max <= 0 ? 0f : 100f * current / max;

            if (_fill != null) _fill.style.width = Length.Percent(percent);
            if (_value != null) _value.text = $"{current} / {max}";
        }

        // the player is the only thing on our own team carrying health
        private static Health FindPlayerHealth()
        {
            foreach (Health health in FindObjectsByType<Health>(FindObjectsSortMode.None))
            {
                if (health.Team == TeamEnum.Player) return health;
            }

            return null;
        }
    }
}
