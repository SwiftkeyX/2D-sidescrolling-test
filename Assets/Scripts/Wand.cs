using SideScroller.Equipments;
using UnityEngine;

namespace SideScroller.Combat
{
    public class Wand : Equipment
    {
        [SerializeField] private Bullet _bulletPrefab;

        public override void Activate(Vector2 direction)
        {
            if (_bulletPrefab == null) return;
    
            Vector2 aim = direction.normalized;
            Vector3 muzzle = transform.position + (Vector3)aim;

            Bullet bullet = Instantiate(_bulletPrefab, muzzle, Quaternion.identity);
            bullet.Launch(aim);
        }
    }
}
