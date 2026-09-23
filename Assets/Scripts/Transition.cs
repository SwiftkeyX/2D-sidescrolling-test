
namespace MagicSchool.Combat.Heroes.States
{
    internal class Transition
    {
        private readonly Player _me;

        public Transition(Player me)
        {
            _me = me;
        }

        // ======================================== transition condition ========================================
        public bool WantsWalk()
        {
            if (_me.MoveInput != 0f) return true;

            return false;
        }
    }
}
