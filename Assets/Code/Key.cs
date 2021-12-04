using UnityEngine;

namespace Code
{
    public class Key : MonoBehaviour
    {
        private static int _collectedKeyCount;
        
        private bool _collected;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player) && !_collected)
            {
                Debug.Log("A key collected");
                _collected = true;
            }
        }

        private void OnKeyCollected()
        {
            _collectedKeyCount++;
            if (_collectedKeyCount == GlobalProperties.Instance.KeyCount)
            {
                Debug.Log("All keys collected");
            }
        }
    }
}