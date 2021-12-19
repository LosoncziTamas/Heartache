using Code.Enemy;
using DG.Tweening;
using UnityEngine;

namespace Code.Hero
{
    public class Aura : MonoBehaviour
    {
        [SerializeField] private HeroProperties _heroProperties;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Collider2D[] _colliders = new Collider2D[16];
        private bool _maxScaleHit;
        private float _lastT;

        public void Grow(float t)
        {
            if (_maxScaleHit)
            {
                return;
            }

            _lastT = t;
            var maxScale = Vector3.one * _heroProperties.AuraScale;
            transform.localScale = maxScale * t;
            if (Mathf.Approximately(t, 1.0f))
            {
                transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
                _maxScaleHit = true;
            }
        }

        public void Emit()
        {
            _maxScaleHit = false;
            transform.DOScale(Vector3.zero, 0.3f);
            var size = Physics2D.OverlapCircleNonAlloc(transform.position, transform.localScale.x, _colliders);
            for (var i = 0; i < size; i++)
            {
                var other = _colliders[i].gameObject;
                if (other.layer == PhysicsUtils.EnemyLayer)
                {
                    other.GetComponent<EnemyController>().PushAway(transform.position, _lastT);
                }
            }
        }
    }
}