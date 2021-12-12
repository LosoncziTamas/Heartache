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
            // TODO: find something more efficient
            _keys.AddRange(FindObjectsOfType<Key>());
            _exit = FindObjectOfType<Exit>();
            _exitMarker.gameObject.SetActive(false);
            foreach (var marker in _markers)
            {
                marker.gameObject.SetActive(false);
            }
            Debug.Assert(_keys.Count <= _markers.Length);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Show compass"))
            {
                Setup();
            }
        }

        private void Update()
        {
            for (var index = 0; index < _keys.Count; index++)
            {
                var marker = _markers[index];
                var key = _keys[index];
                if (key.isActiveAndEnabled)
                {
                    marker.gameObject.SetActive(true);
                    var direction = (Vector2)(key.transform.position - transform.position);
                    if (direction.magnitude < _heroProperties.CompassRange)
                    {
                        marker.gameObject.SetActive(false);
                    }
                    else
                    {
                        marker.localPosition = direction.normalized * _heroProperties.CompassRange;
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
                var direction = (Vector2)(_exit.transform.position - transform.position);
                if (direction.magnitude < _heroProperties.CompassRange)
                {
                    _exitMarker.gameObject.SetActive(false);
                }
                else
                {
                    _exitMarker.localPosition = direction.normalized * _heroProperties.CompassRange;
                }
            }
        }
    }
}