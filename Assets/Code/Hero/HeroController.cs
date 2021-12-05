using System;
using UnityEngine;

namespace Code.Hero
{
    public class HeroController : MonoBehaviour
    {
        public static HeroController Instance { get; private set; }

        [SerializeField] private HeroProperties _heroProperties;
        [SerializeField] private Bullet _bulletPrefab;

        private Rigidbody2D _rigidbody2D;
        private Camera _camera;

        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _camera = Camera.main;
        }

        private bool _isDown;

        private void OnGUI()
        {
            if (_isDown)
            {
                GUILayout.Label("Is Down");
            }
            else
            {
                GUILayout.Label("Not Down");
            }
        }

        private void OnDrawGizmos()
        {
            var mouseScreenPos = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Gizmos.DrawLine(transform.position, mouseWorldPos);                
        }

        private void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var shoot = Input.GetMouseButtonDown(0);
            
            if (Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0)
            {
                _isDown = true;
                var delta = new Vector2(horizontal, vertical);
                var deltaNormalized = delta.normalized * _heroProperties.Speed * Time.deltaTime;
                _rigidbody2D.velocity = deltaNormalized;
            }
            else
            {
                _isDown = false;
                _rigidbody2D.velocity = Vector2.zero;
            }

            if (shoot)
            {
                var bullet = Instantiate(_bulletPrefab, transform.position, transform.rotation, null);
                var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction =mouseWorldPos - transform.position;
                bullet.Launch(direction.normalized);
            }
            
        }
    }
}
