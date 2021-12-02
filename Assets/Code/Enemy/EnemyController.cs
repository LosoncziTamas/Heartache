using Code.Hero;
using UnityEngine;

namespace Code.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyProperties _enemyProperties;
        [SerializeField] private Rigidbody2D _rigidbody2D;

        private bool _heroWithinRange;
        private HeroController _hero;
        private Vector2 _direction;
        
        private void Start()
        {
            _hero = HeroController.Instance;
        }
        
        private void FixedUpdate()
        {
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
            if (other.gameObject.CompareTag(Tags.Player))
            {
                _heroWithinRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player))
            {
                _heroWithinRange = false;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player))
            {
                _heroWithinRange = true;
            }
        }
    }
}