using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        private GlobalProperties _globalProperties;
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _globalProperties = GlobalProperties.Instance;
        }

        public void Launch(Vector2 direction)
        {
            _rigidbody2D.AddRelativeForce(direction * _globalProperties.BulletLaunchForce, ForceMode2D.Impulse);
            Invoke(nameof(DestroySelf), _globalProperties.BulletLifeDurationInSeconds);
        }

        private void DestroySelf()
        {
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var otherLayer = other.gameObject.layer;
            if (otherLayer == LayerMask.NameToLayer("Wall"))
            {
                DestroySelf();
            }
            else if (otherLayer == LayerMask.NameToLayer("Enemy"))
            {
                // DestroySelf();
                // TODO something
            }
        }
    }
}
