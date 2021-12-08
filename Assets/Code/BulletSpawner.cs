using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class BulletSpawner : MonoBehaviour
    {
        private const int InitialPoolSize = 8;
        
        [SerializeField] private Bullet _bulletPrefab;
        
        private readonly List<Bullet> _bullets = new List<Bullet>();

        private void Awake()
        {
            for (var i = 0; i < InitialPoolSize; i++)
            {
                var bullet = Instantiate(_bulletPrefab);
                bullet.BulletSpawner = this;
                Return(bullet);
            }
        }

        public Bullet Spawn(Vector3 position)
        {
            var item = _bullets[0];
            item.gameObject.SetActive(true);
            var bulletTransform = item.transform;
            bulletTransform.SetParent(null);
            bulletTransform.position = position;
            _bullets.RemoveAt(0);
            return item;
        }

        public void Return(Bullet bullet)
        {
            bullet.transform.SetParent(transform);
            bullet.gameObject.SetActive(false);
            _bullets.Add(bullet);
        }
    }
}