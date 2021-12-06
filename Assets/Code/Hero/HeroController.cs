using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Hero
{
    public class HeroController : MonoBehaviour
    {
        public static HeroController Instance { get; private set; }

        [SerializeField] private HeroProperties _heroProperties;
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Image _bulletPower;

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

        private IEnumerator FillImageWhileButtonIsDown()
        {
            var maxTime = GlobalProperties.Instance.BulletPressToMaxPowerDurationInSeconds;
            var t = 0f;
            while (Input.GetMouseButton(0))
            {
                _bulletPower.fillAmount = GlobalProperties.Instance.BulletFillCurve.Evaluate(Mathf.Clamp01(t / maxTime));
                t += Time.deltaTime;
                yield return null;
            }
        }
        
        private void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var buttonDown = Input.GetMouseButtonDown(0);
            var buttonUp = Input.GetMouseButtonUp(0);
            
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

            if (buttonDown)
            {
                StartCoroutine(FillImageWhileButtonIsDown());
            }
            else if (buttonUp)
            {
                var bullet = Instantiate(_bulletPrefab, transform.position, transform.rotation, null);
                var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction =mouseWorldPos - transform.position;
                bullet.Launch(direction.normalized, _bulletPower.fillAmount);
                _bulletPower.fillAmount = 0.0f;
            }
        }
    }
}
