using UnityEngine;
using UnityEngine.InputSystem;

namespace SideScroller.Input
{
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

        // to make not "reload domain" option in Enter play mode work
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _actions = null;

        // keyboard
        public static float MoveAxis => Actions.Player.Move.ReadValue<Vector2>().x;
        public static bool JumpPressedThisFrame => Actions.Player.Jump.WasPressedThisFrame();
        public static bool AttackPressedThisFrame => Actions.Player.Attack.WasPressedThisFrame();
        public static bool InventoryPressedThisFrame => Actions.Player.Inventory.WasPressedThisFrame();
        public static bool InteractPressedThisFrame => Actions.Player.Interact.WasPressedThisFrame();
        
        // left/right click 
        public static bool IsPointerDown => Mouse.current != null && Mouse.current.leftButton.isPressed;
        public static bool DragPressedThisFrame => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        public static bool DragReleasedThisFrame => Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;

        public static Vector2 PointerScreenPosition => Mouse.current == null ? Vector2.zero : Mouse.current.position.ReadValue();
    }
}
