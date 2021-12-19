using UnityEngine;

namespace Code
{
    public static class PhysicsUtils
    {
        public static readonly ContactFilter2D NoFilter = new ContactFilter2D().NoFilter();
        public static readonly int EnemyLayer = LayerMask.NameToLayer("Enemy");
        public static readonly int HeroLayer = LayerMask.NameToLayer("Hero");
        public static readonly int HeroAndEnemyInteractableLayer = LayerMask.NameToLayer("Hero And Enemy Interactable");
        
    }
}