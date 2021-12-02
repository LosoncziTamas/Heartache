using UnityEngine;

namespace Code.Enemy
{
    [CreateAssetMenu(fileName = "Enemy Properties", menuName = "Scriptable Objects/Enemy Properties", order = 2)]
    public class EnemyProperties : ScriptableObject
    {
        public float Speed;
    }
}