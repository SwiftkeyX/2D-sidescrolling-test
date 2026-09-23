using UnityEngine;

namespace SideScroller.Combat
{
    // Drive forward in a straight line and cleans itself up.
    [RequireComponent(typeof(Hitbox))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 12f;
        [SerializeField] private float _lifeTime = 5f;

        private Vector2 _direction = Vector2.right;

        public void Launch(Vector2 direction)
        {
            _direction = direction.normalized;
        }

        void Awake()
        {
            GetComponent<Hitbox>().Hit += OnHit;
        }

        void Start()
        {
            Destroy(gameObject, _lifeTime);
        }

        void Update()
        {
            transform.Translate(_direction * (_speed * Time.deltaTime), Space.World);
        }

        // OnHit, destroy itself
        private void OnHit(IDamageable target)
        {
            Destroy(gameObject);
        }
    }
}
