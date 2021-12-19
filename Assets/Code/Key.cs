using Code.Gui;
using UnityEngine;

namespace Code
{
    public class Key : MonoBehaviour
    {
        public static int CollectedKeyCount { get; set; }

        public static bool KeysAreCollected => CollectedKeyCount == GlobalProperties.Instance.KeyCountPerLevel;
        
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
            CollectedKeyCount++;
            if (KeysAreCollected)
            {
                MessagePanel.Instance.ShowMessage("All fragments collected! Hurry up and move on to the next level.");
            }
            else
            {
                MessagePanel.Instance.ShowMessage( $"A fragment collected. {GlobalProperties.Instance.KeyCountPerLevel - CollectedKeyCount} more to go!");
            }
            gameObject.SetActive(false);
        }
    }
}