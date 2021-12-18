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

        private bool _heroWithinRange;
        private HeroController _hero;
        private Vector2 _direction;
        private bool _dead;

        private void Start()
        {
            _hero = HeroController.Instance;
        }
        
        private void FixedUpdate()
        {
            if (_dead)
            {
                _rigidbody2D.velocity = Vector2.zero;
                return;
            }

            _direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            _rigidbody2D.velocity = _direction.normalized * _enemyProperties.Speed * Time.fixedDeltaTime;
            return;
            if (_heroWithinRange && !Room.FocusedRoom.CameraIsMoving)
            {
                _direction = _hero.transform.position - transform.position;
                _rigidbody2D.velocity = _direction.normalized * _enemyProperties.Speed * Time.fixedDeltaTime;
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
    }
}