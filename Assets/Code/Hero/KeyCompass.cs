using System.Collections.Generic;
using UnityEngine;

namespace Code.Hero
{
    public class KeyCompass : MonoBehaviour
    {
        [SerializeField] private Transform[] _markers;
        private readonly List<Key> _keys = new List<Key>();

        public void Setup()
        {
            _keys.Clear();
            // TODO: find something more efficient
            _keys.AddRange(FindObjectsOfType<Key>());
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
                    marker.localPosition = direction.normalized;
                }
                else
                {
                    marker.gameObject.SetActive(false);
                }
            }
        }
    }
}