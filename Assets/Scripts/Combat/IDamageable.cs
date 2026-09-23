namespace SideScroller.Combat
{
    public interface IDamageable
    {
        TeamEnum Team { get; }
        void TakeDamage(int amount);
    }
}
