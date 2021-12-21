#undef DEBUG

using Code.Hero;
using Code.Rooms;
using DG.Tweening;
using UnityEngine;

namespace Code.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private static readonly Vector3 FallingOffset = new Vector3(-0.5f, -0.5f);
        private static readonly int FallingDown = Animator.StringToHash("Falling Down");
        
        [SerializeField] private EnemyProperties _enemyProperties;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Animator _animator;
        [SerializeField] private BoolReference _playerEscaped;
        
        private bool _heroWithinRange;
        private HeroController _hero;
        private Vector2 _direction;
        private bool _dead;
        private bool _pushing;

        private void Start()
        {
            _hero = HeroController.Instance;
        }
        
        private void Update()
        {
            if (_dead || !_hero.FinishedIntro || _hero.IsDead || _playerEscaped.Value)
            {
                _rigidbody2D.velocity = Vector2.zero;
                return;
            }

            if (_pushing)
            {
                return;
            }
#if DEBUG
            _direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            _rigidbody2D.velocity = _direction.normalized * _enemyProperties.Speed * Time.fixedDeltaTime;
            return;
#endif
            if (_heroWithinRange && !Room.FocusedRoom.CameraIsMoving)
            {
                _direction = _hero.transform.position - transform.position;
                _rigidbody2D.velocity = _direction.normalized * _enemyProperties.Speed * Time.deltaTime;
            }
            else
            {
                _rigidbody2D.velocity = Vector2.zero;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.MainCamera))
            {
                _heroWithinRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.MainCamera))
            {
                _heroWithinRange = false;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.MainCamera))
            {
                _heroWithinRange = true;
            }
        }

        public void FallDown(Vector3 position)
        {
            _animator.SetBool(FallingDown, true);
            var fallingPos = position + FallingOffset;
            var distance = Vector2.Distance(transform.position, fallingPos);
            var duration = (distance / 1.0f) * 0.4f;
            transform.DOMove(fallingPos, duration);
            _dead = true;
        }

        public void PushAway(Vector2 from, float strength)
        {
            var push = ((Vector2)transform.position - from).normalized;
            _rigidbody2D.AddRelativeForce(push * strength * _enemyProperties.PushAwayScale, ForceMode2D.Impulse);
            _pushing = true;
            CancelInvoke(nameof(ResetPush));
            Invoke(nameof(ResetPush), 0.3f);
        }

        private void ResetPush()
        {
            _pushing = false;
        }
    }
}