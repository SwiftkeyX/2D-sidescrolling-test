using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SideScroller.UI
{
    // Picker answer: what is the pointer point at?
    internal static class Picker
    {
        // screen pixel => panel position
        public static Vector2 ToPanel(IPanel panel, Vector2 screenPos)
        {
            Vector2 flipped = new Vector2(screenPos.x, Screen.height - screenPos.y);
            return RuntimePanelUtils.ScreenToPanel(panel, flipped);
        }

        // context: panel.Pick return the top-most element, which may be a child of what we want.
        // At() walk up the parents to ensure the picker return one of the candidates. -1 = none
        public static int At(IPanel panel, Vector2 panelPos, VisualElement[] candidates)
        {
            VisualElement hit = panel?.Pick(panelPos);

            while (hit != null)
            {
                int index = Array.IndexOf(candidates, hit);
                if (index >= 0) return index;

                hit = hit.parent;
            }

            return -1;
        }
    }
}
