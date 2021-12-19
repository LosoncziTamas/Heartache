using Code.Enemy;
using Code.Hero;
using UnityEngine;

namespace Code
{
    public class Trap : MonoBehaviour
    {
        private static readonly int Enabled = Animator.StringToHash("Enabled");
        
        [SerializeField] private Animator _animator;
        [SerializeField] private FloatReference _trapActivationTimeInSeconds;
        
        public bool Activated { get; private set; }

        private float _enterStart;
        private float _enemyEnterStart;

        private void OnTriggerEnter2D(Collider2D other)
        {
            var otherLayer = other.gameObject.layer;
            var isHero = otherLayer == PhysicsUtils.HeroLayer;
            var isEnemy = otherLayer == PhysicsUtils.EnemyLayer;
            if (isHero)
            {
                _enterStart = Time.time;
            }
            else if (isEnemy)
            {
                _enemyEnterStart = Time.time;
            }
            CheckFalling(other.gameObject, isHero, isEnemy);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            var otherLayer = other.gameObject.layer;
            var isHero = otherLayer == PhysicsUtils.HeroLayer;
            var isEnemy = otherLayer == PhysicsUtils.EnemyLayer;
            CheckFalling(other.gameObject, isHero, isEnemy);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var otherLayer = other.gameObject.layer;
            var isHero = otherLayer == PhysicsUtils.HeroLayer;
            var isEnemy = otherLayer == PhysicsUtils.EnemyLayer;
            if (isHero)
            {
                _enterStart = -1;
            }
            else if (isEnemy)
            {
                _enemyEnterStart = -1;
            }
        }

        private void CheckFalling(GameObject other, bool isHero, bool isEnemy)
        {
            if (isHero || isEnemy)
            {
                if (Activated)
                {
                    if (isHero && _enterStart > 0)
                    {
                        if (Time.time - _enterStart > _trapActivationTimeInSeconds.Value)
                        {
                            HeroController.Instance.FallDown(transform.position);
                        }
                    }
                    else if (isEnemy && _enemyEnterStart > 0)
                    {
                        if (Time.time - _enemyEnterStart > _trapActivationTimeInSeconds.Value)
                        {
                            other.GetComponent<EnemyController>().FallDown(transform.position);
                        }
                    }
                    return;
                }
                _animator.SetBool(Enabled, true);
                Activated = true;
            }
        }
    }
}