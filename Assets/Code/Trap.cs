using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class Trap : MonoBehaviour
    {
        private static readonly int Enabled = Animator.StringToHash("Enabled");
        
        [SerializeField] private Animator _animator;

        public void OnGUI()
        {
            if (GUILayout.Button("Activate Trap"))
            {
                _animator.SetBool(Enabled, true);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var otherLayer = other.gameObject.layer;        
            if (otherLayer == LayerMask.NameToLayer("Hero") || otherLayer == LayerMask.NameToLayer("Enemy"))
            {
                _animator.SetBool(Enabled, true);
                Debug.Log("OnTriggerEnter2D");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Debug.Log("OnTriggerExit2D");
        }
    }
}