using UnityEngine;
using UnityEngine.InputSystem;

namespace SideScroller.Input
{
    // Player can: 
    // 1) left click to drag hero/items.
    // 2) right click to inspect hero/items.
    // 3) spacebar to interact with game's stage e.g. start combat, retry, continue to next stage
    // 4) R to quick-retry
    public static class PlayerInputSystem
    {
        private static InputSystem_Actions _actions;
        private static InputSystem_Actions Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new InputSystem_Actions();
                    _actions.Player.Enable();
                }

                return _actions;
            }
        }

        public static float MoveAxis => Actions.Player.Move.ReadValue<Vector2>().x;

        public static bool IsPointerDown => Mouse.current.leftButton.isPressed;
        public static bool DragPressedThisFrame => Mouse.current.leftButton.wasPressedThisFrame;
        public static bool DragReleasedThisFrame => Mouse.current.leftButton.wasReleasedThisFrame;
        public static bool SpacePressedThisFrame => Keyboard.current.spaceKey.wasPressedThisFrame;
        public static bool InspectPressedThisFrame => Mouse.current.rightButton.wasPressedThisFrame;
        public static bool RestartPressedThisFrame => Keyboard.current.rKey.wasPressedThisFrame;
        public static Vector2 PointerScreenPosition => Mouse.current.position.ReadValue();
    }
}
