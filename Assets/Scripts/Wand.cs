using SideScroller.Equipments;
using UnityEngine;

namespace SideScroller.Combat
{
    public class Wand : Equipment
    {
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Vector2 _muzzleOffset = new Vector2(0.6f, 0.1f);

        public override void Activate(float facing)
        {
            if (_bulletPrefab == null) return;

            Vector3 muzzle = transform.position + new Vector3(_muzzleOffset.x * facing, _muzzleOffset.y, 0f);

            Bullet bullet = Instantiate(_bulletPrefab, muzzle, Quaternion.identity);
            bullet.Launch(new Vector2(facing, 0f));
        }
    }
}
