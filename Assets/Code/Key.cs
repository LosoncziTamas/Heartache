using UnityEngine;

namespace Code
{
    public class Key : MonoBehaviour
    {
        public static int CollectedKeyCount { get; private set; }
        
        private bool _collected;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player) && !_collected)
            {
                OnKeyCollected();
            }
        }

        private void OnKeyCollected()
        {
            _collected = true;
            Debug.Log("A key collected.");
            CollectedKeyCount++;
            if (CollectedKeyCount == GlobalProperties.Instance.KeyCount)
            {
                Debug.Log("All keys collected.");
            }
        }
    }
}