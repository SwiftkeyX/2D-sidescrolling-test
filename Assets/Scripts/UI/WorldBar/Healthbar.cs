using SideScroller.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace SideScroller.UI
{
    // World-space health bar - green for the player's side, red for the other one.
    internal class Healthbar : WorldBar
    {
        private Image _image;

        // ====================================== override ======================================
        // fill slider with the health value
        protected override float Fill => (float)_health.Current / _health.Max;

        // ====================================== life cycle ======================================
        protected override void Awake()
        {
            base.Awake();

            if (_slider != null && _slider.fillRect != null)
            {
                _image = _slider.fillRect.GetComponent<Image>();
            }
        }

        void Start()
        {
            if (_image == null || _health == null) return;

            if (_health.Team == TeamEnum.Player) _image.color = Color.green;

            else if (_health.Team == TeamEnum.Enemy) _image.color = Color.red;
        }
    }
}
