using UnityEngine;

namespace Code.Enemy
{
    [CreateAssetMenu]
    public class EnemyProperties : ScriptableObject
    {
        public float Speed;
        public float PushAwayScale;
    }
}