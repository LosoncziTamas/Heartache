using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CheckOpeningTraversable : MonoBehaviour
    {
        private static readonly ContactFilter2D NoFilter = new ContactFilter2D().NoFilter();
        
        private BoxCollider2D _boxCollider;
        
        [SerializeField] private GameObject _wallChunk;
        [SerializeField] private bool _horizontalOpening;

        private readonly Collider2D[] _contacts = new Collider2D[4];

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        public bool OpeningIsTraversable()
        {
            var overlapCount = _boxCollider.OverlapCollider(NoFilter, _contacts);
            for (var i = 0; i < overlapCount; i++)
            {
                var contact = _contacts[i];
                if (contact.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    return false;
                }
            }
            return true;
        }

        public void CloseOpening()
        {
            if (_horizontalOpening)
            {
                _wallChunk.transform.position += Vector3.left * 0.5f;
                _wallChunk.transform.localScale = Vector3.one + Vector3.right * 5.5f;
            }
            else
            {
                _wallChunk.transform.position += Vector3.up * 0.5f;
                _wallChunk.transform.localScale = Vector3.one + Vector3.up * 5.5f;
            }
        }
    }
}
