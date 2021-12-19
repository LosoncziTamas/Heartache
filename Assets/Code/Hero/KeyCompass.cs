using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Hero
{
    public class KeyCompass : MonoBehaviour
    {
        public static KeyCompass Instance { get; private set; }
        
        [SerializeField] private Transform[] _markers;
        [SerializeField] private Transform _exitMarker;
        [SerializeField] private HeroProperties _heroProperties;
        private readonly List<Key> _keys = new List<Key>();
        private Exit _exit;

        private void Awake()
        {
            Debug.Assert(Instance == null);
            Instance = this;
        }

        public void Setup()
        {
            _keys.Clear();
            var keys = FindObjectsOfType<Key>();
            _keys.AddRange(keys);
            _exit = FindObjectOfType<Exit>();
            _exitMarker.gameObject.SetActive(false);
            foreach (var marker in _markers)
            {
                marker.gameObject.SetActive(false);
            }
            Debug.Assert(_keys.Count <= _markers.Length);
        }
        
        private void Update()
        {
            for (var index = 0; index < _keys.Count; index++)
            {
                var marker = _markers[index];
                var key = _keys[index];
                if (key.isActiveAndEnabled && !key.Collected)
                {
                    var targetPos = (Vector2)key.transform.position - Vector2.one * 0.5f;
                    var startPos = (Vector2)transform.position;    
                    marker.gameObject.SetActive(true);
                    var direction = targetPos - startPos;
                    var distance = Vector2.Distance(targetPos, startPos);
                    if (distance < _heroProperties.CompassRange * 2.0f)
                    {
                        marker.gameObject.SetActive(false);
                    }
                    else
                    {
                        var dirNormalized = direction.normalized;
                        marker.localPosition = dirNormalized * _heroProperties.CompassRange;
                        var angle = Mathf.Atan2(dirNormalized.y, dirNormalized.x) * Mathf.Rad2Deg;
                        marker.localRotation = Quaternion.Euler(0, 0, angle);
                    }
                }
                else
                {
                    marker.gameObject.SetActive(false);
                }
            }

            if (_exit && Key.KeysAreCollected)
            {
                _exitMarker.gameObject.SetActive(true);
                var targetPos = (Vector2)_exit.transform.position - Vector2.one * 0.5f;
                var startPos = (Vector2)transform.position;    
                var direction = targetPos - startPos;
                var distance = Vector2.Distance(targetPos, startPos);
                if (distance < _heroProperties.CompassRange * 2.0f)
                {
                    _exitMarker.gameObject.SetActive(false);
                }
                else
                {
                    _exitMarker.localPosition = direction.normalized * _heroProperties.CompassRange;
                    var dirNormalized = direction.normalized;
                    var angle = Mathf.Atan2(dirNormalized.y, dirNormalized.x) * Mathf.Rad2Deg;
                    _exitMarker.localRotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }
    }
}