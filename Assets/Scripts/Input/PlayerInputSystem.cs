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

        public static float MoveAxis => Actions.Player.Move.ReadValue<Vector2>().x;
        public static bool JumpPressedThisFrame => Actions.Player.Jump.WasPressedThisFrame();
        public static bool AttackPressedThisFrame => Actions.Player.Attack.WasPressedThisFrame();
        public static Vector2 PointerScreenPosition => Mouse.current == null ? Vector2.zero : Mouse.current.position.ReadValue();
    }
}
