using Code.Gui;
using UnityEngine;

namespace Code
{
    public class Key : MonoBehaviour
    {
        public static int CollectedKeyCount { get; set; }

        public static bool KeysAreCollected => CollectedKeyCount == GlobalProperties.Instance.KeyCount;
        
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
            MessagePanel.Instance.ShowMessage("A key collected.");
            CollectedKeyCount++;
            if (KeysAreCollected)
            {
                MessagePanel.Instance.ShowMessage("All keys collected.");
            }
            gameObject.SetActive(false);
        }
    }
}