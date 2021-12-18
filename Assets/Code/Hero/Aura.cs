using Code.Enemy;
using DG.Tweening;
using UnityEngine;

namespace Code.Hero
{
    public class Aura : MonoBehaviour
    {
        [SerializeField] private HeroProperties _heroProperties;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private CircleCollider2D _circleCollider2D;

        private bool _maxScaleHit;

        public void Grow(float t)
        {
            if (_maxScaleHit)
            {
                return;
            }
            _circleCollider2D.radius = 0.0f;
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
            var overlaps = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x * 0.5f);
            for (var i = 0; i < overlaps.Length; i++)
            {
                var other = overlaps[i].gameObject;
                if (other.layer == PhysicsUtils.EnemyLayer)
                {
                    Debug.Log("Enemy overlap");
                    other.GetComponent<EnemyController>().PushAway(transform.position, 1.0f);
                }
            }
        }
    }
}