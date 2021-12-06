using UnityEngine;

namespace Code
{
    public class OnScreenCursor : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        public void Update()
        {
            Vector2 targetPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            transform.position = targetPos;
        }
    }
}