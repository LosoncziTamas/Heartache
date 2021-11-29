using System;
using UnityEngine;

namespace Code.Hero
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField] private HeroProperties _heroProperties;

        private Rigidbody2D _rigidbody2D;
        
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0)
            {
                var delta = new Vector2(horizontal, vertical);
                var deltaNormalized = delta.normalized * _heroProperties.Speed * Time.deltaTime;
                _rigidbody2D.velocity = deltaNormalized;
            }
            else
            {
                _rigidbody2D.velocity = Vector2.zero;
            }

            /*
            var delta = new Vector3(horizontal, vertical, _localZ).normalized;
            delta *= _heroProperties.Speed * Time.deltaTime;
            transform.localPosition += delta;*/

        }
    }
}
