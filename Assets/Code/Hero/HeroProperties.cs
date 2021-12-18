using UnityEngine;

namespace Code.Hero
{
    [CreateAssetMenu(fileName = "Hero Properties", menuName = "Scriptable Objects/Hero Properties", order = 2)]
    public class HeroProperties : ScriptableObject
    {
        public float Speed;
        public float CompassRange;
        public float AuraScale;
    }
}