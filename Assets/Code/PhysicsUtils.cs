using UnityEngine;

namespace Code
{
    public static class PhysicsUtils
    {
        public static readonly ContactFilter2D NoFilter = new ContactFilter2D().NoFilter();
        public static int EnemyLayer = LayerMask.NameToLayer("Enemy");
        public static int HeroLayer = LayerMask.NameToLayer("Enemy");
        
    }
}