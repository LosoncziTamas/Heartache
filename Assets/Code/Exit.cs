using System;
using UnityEngine;

namespace Code
{
    public class Exit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player))
            {
                Debug.Log("Exit");
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            
        }
    }
}
