using System.Collections;
using Code.Gui;
using Code.Rooms;
using DG.Tweening;
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
        private static readonly Vector3 FallingOffset = new Vector3(-0.5f, -0.5f);

        [SerializeField] private HeroProperties _heroProperties;
        [SerializeField] private Aura _aura;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Rigidbody2D _rigidbody2D;
        private LevelGenerator _levelGenerator;
        private Camera _camera;
        
        private static readonly int FacingDirectionProperty = Animator.StringToHash("Facing Direction");
        private static readonly int MovingProperty = Animator.StringToHash("Moving");
        private static readonly int Death = Animator.StringToHash("Death");
        private static readonly int FallingDown = Animator.StringToHash("Falling Down");
        
        private FacingDirection _facingDirection = FacingDirection.Front;
        private bool _moving = false;
        private bool _animStateChanged;
        private bool _isDead;
        private bool _isFallingDown;

        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _levelGenerator = FindObjectOfType<LevelGenerator>();
            _camera = Camera.main;
            UpdateAnim();
        }

        private void UpdateAnim(bool isFallingDown = false)
        {
            _animator.SetBool(Death, _isDead);
            _animator.SetBool(FallingDown, isFallingDown);
            _animator.SetInteger(FacingDirectionProperty, (int)_facingDirection);
            _animator.SetBool(MovingProperty, _moving);
        }
        
        private IEnumerator FillImageWhileButtonIsDown()
        {
            var maxTime = GlobalProperties.Instance.BulletPressToMaxPowerDurationInSeconds;
            var t = 0f;
            while (Input.GetMouseButton(0))
            {
                _aura.Grow(GlobalProperties.Instance.BulletFillCurve.Evaluate(Mathf.Clamp01(t / maxTime)));
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

            if (_isDead)
            {
                _rigidbody2D.velocity = Vector2.zero;
                if (Input.GetButton("Submit"))
                {
                    _isDead = false;
                    UpdateAnim();
                    _levelGenerator.RestartGame(transform);
                }
                return;
            }
            
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
                _aura.Emit();
            }

            if (_animStateChanged)
            {
                UpdateAnim();
            }
        }

        public void Die()
        {
            _isDead = true;
            UpdateAnim();
            MessagePanel.Instance.ShowMessage("Your time is up... Press enter to restart.");
        }

        public void FallDown(Vector3 trapPosition)
        {
            if (_isDead)
            {
                return;
            }
            _isDead = true;
            transform.DOMove(trapPosition + FallingOffset, 0.4f);
            UpdateAnim(isFallingDown: true);
        }
    }
}
