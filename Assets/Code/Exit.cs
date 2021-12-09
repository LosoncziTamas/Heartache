using System;
using UnityEngine;

namespace Code
{
    public class Exit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player) && Key.CollectedKeyCount == GlobalProperties.Instance.KeyCount)
            {
                Debug.Log("Level Complete");
            }
        }
    }
}
