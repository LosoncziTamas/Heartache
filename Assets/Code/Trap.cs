using System;
using System.Collections.Generic;
using Code.Hero;
using UnityEngine;

namespace Code
{
    public class Trap : MonoBehaviour
    {
        private static readonly int Enabled = Animator.StringToHash("Enabled");
        
        [SerializeField] private Animator _animator;
        [SerializeField] private FloatReference _trapActivationTimeInSeconds;
        
        public bool Activated { get; private set; }

        private float _enterStart;

        private void OnTriggerEnter2D(Collider2D other)
        {
            _enterStart = Time.time;
            CheckFalling(other.gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            CheckFalling(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _enterStart = -1;
        }

        private void CheckFalling(GameObject other)
        {
            var otherLayer = other.gameObject.layer;
            var isHero = otherLayer == LayerMask.NameToLayer("Hero");
            var isEnemy = otherLayer == LayerMask.NameToLayer("Enemy");
            if (isHero || isEnemy)
            {
                if (Activated)
                {
                    if (isHero && _enterStart > 0)
                    {
                        if (Time.time - _enterStart > _trapActivationTimeInSeconds.Value)
                        {
                            HeroController.Instance.FallDown(transform.position);
                        }
                    }
                    return;
                }
                _animator.SetBool(Enabled, true);
                Activated = true;
            }
        }
    }
}