using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour
    {
        public BulletSpawner BulletSpawner { get; set; }

        private Rigidbody2D _rigidbody2D;
        private GlobalProperties _globalProperties;
        
        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _globalProperties = GlobalProperties.Instance;
        }

        public void Launch(Vector2 direction, float additionalPower)
        {
            var additional = _globalProperties.BulletLaunchForceAdditional * additionalPower;
            _rigidbody2D.AddRelativeForce(direction * (_globalProperties.BulletLaunchForceBase + additional), ForceMode2D.Impulse);
            Invoke(nameof(DestroySelf), _globalProperties.BulletLifeDurationInSeconds);
        }

        private void DestroySelf()
        {
            CancelInvoke(nameof(DestroySelf));
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0;
            transform.rotation = Quaternion.identity;
            BulletSpawner.Return(this);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var otherLayer = other.gameObject.layer;
            if (otherLayer == LayerMask.NameToLayer("Wall"))
            {
                DestroySelf();
            }
            else if (otherLayer == PhysicsUtils.EnemyLayer)
            {
                // DestroySelf();
                // TODO something
            }
        }
    }
}
