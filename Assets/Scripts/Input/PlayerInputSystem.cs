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

        /// <summary>
        /// Enter Play Mode Settings is set to reload the scene but not the domain, so statics
        /// outlive a play session. Without this, _actions comes back pointing at the action set
        /// the Input System disabled on exit, and the getter above skips re-enabling it because
        /// it is not null - input is simply dead from the second Play onwards, silently.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => _actions = null;

        public static float MoveAxis => Actions.Player.Move.ReadValue<Vector2>().x;
        public static bool JumpPressedThisFrame => Actions.Player.Jump.WasPressedThisFrame();
        public static bool AttackPressedThisFrame => Actions.Player.Attack.WasPressedThisFrame();
        public static Vector2 PointerScreenPosition => Mouse.current == null ? Vector2.zero : Mouse.current.position.ReadValue();
    }
}
