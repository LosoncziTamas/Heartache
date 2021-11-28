using System;
using UnityEngine;

namespace Code.Hero
{
    public class HeroController : MonoBehaviour
    {
        [SerializeField] private HeroProperties _heroProperties;

        private float _localZ;

        private void Start()
        {
            _localZ = transform.localPosition.z;
        }

        private void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var delta = new Vector3(horizontal, vertical, _localZ).normalized;
            delta *= _heroProperties.Speed * Time.deltaTime;
            transform.localPosition += delta;
        }
    }
}
