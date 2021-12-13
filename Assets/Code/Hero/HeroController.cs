using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Hero
{
    public class HeroController : MonoBehaviour
    {

        private enum FacingDirection
        {
            Front = 1,
            Back = -1
        };
        
        public static HeroController Instance { get; private set; }

        [SerializeField] private HeroProperties _heroProperties;
        [SerializeField] private Image _bulletPower;
        [SerializeField] private BulletSpawner _bulletSpawner;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Rigidbody2D _rigidbody2D;
        private Camera _camera;
        
        private static readonly int FacingDirectionProperty = Animator.StringToHash("Facing Direction");
        private static readonly int MovingProperty = Animator.StringToHash("Moving");

        private FacingDirection _facingDirection = FacingDirection.Front;
        private bool _moving = false;
        private bool _animStateChanged;
        
        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _camera = Camera.main;
            UpdateAnim();
        }

        private void UpdateAnim()
        {
            _animator.SetInteger(FacingDirectionProperty, (int)_facingDirection);
            _animator.SetBool(MovingProperty, _moving);
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
                var delta = new Vector2(horizontal, vertical);
                var normalizedDelta = delta.normalized;
                var velocity = delta.normalized * _heroProperties.Speed * Time.deltaTime;
                _rigidbody2D.velocity = velocity;

                var movingUp = normalizedDelta.y > 0;
                _animStateChanged = !_moving || movingUp && _facingDirection == FacingDirection.Front || !movingUp && _facingDirection == FacingDirection.Back;

                _spriteRenderer.flipX = normalizedDelta.x < 0;
                
                _moving = true;
                _facingDirection = movingUp ? FacingDirection.Back : FacingDirection.Front;
            }
            else
            {
                _rigidbody2D.velocity = Vector2.zero;

                _animStateChanged = _moving;
                _moving = false;
            }

            if (buttonDown)
            {
                StartCoroutine(FillImageWhileButtonIsDown());
            }
            else if (buttonUp)
            {
                var bullet = _bulletSpawner.Spawn(transform.position);
                var mouseWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction =mouseWorldPos - transform.position;
                bullet.Launch(direction.normalized, _bulletPower.fillAmount);
                _bulletPower.fillAmount = 0.0f;
            }

            if (_animStateChanged)
            {
                UpdateAnim();
            }
        }
    }
}
