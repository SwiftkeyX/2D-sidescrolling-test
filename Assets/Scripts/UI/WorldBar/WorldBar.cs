using SideScroller.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace SideScroller.UI
{
    /// <summary>
    /// A world-space bar floating over a enemy's head.
    /// </summary>

    /// ExecuteAlways lets the offset be tweaked live in edit mode.
    [ExecuteAlways]
    internal abstract class WorldBar : MonoBehaviour
    {
        [SerializeField] protected Slider _slider;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 1f, 0f);

        protected Stat _health;

        // ====================================== abstract ======================================
        // what this bar reads off the health it follows. range from 0 - 1.
        protected abstract float Fill { get; }

        // to hide/show the bar
        protected virtual bool IsShown => true;

        // helper
        private bool? _wasShown;

        // ====================================== setter ======================================
        // set offset to the bar away from its owner
        public void SetOffset(Vector3 offset) => _offset = offset;

        // ====================================== life cycle ======================================
        protected virtual void Awake() => _health = GetComponentInParent<Stat>();

        // update bar position/visibility
        protected virtual void LateUpdate()
        {
            if (_health == null) _health = GetComponentInParent<Stat>();
            if (_health == null) return;

            // update position 
            transform.position = _health.transform.position + _offset;

            // guard
            if (!Application.isPlaying) return;

            bool shown = IsShown;

            // only touch the graphics when the answer actually changed
            if (shown != _wasShown)
            {
                _wasShown = shown;
                ShowGraphics(shown);
            }

            // update worldbar slider
            if (shown && _slider != null) _slider.value = Fill;
        }

        // ====================================== private ======================================
        // turn on/off the bar
        private void ShowGraphics(bool shown)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(shown);
        }
    }
}
