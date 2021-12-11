using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CheckOpeningTraversable : MonoBehaviour
    {
        private static readonly ContactFilter2D NoFilter = new ContactFilter2D().NoFilter();
        
        private BoxCollider2D _boxCollider;
        
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
    }
}
