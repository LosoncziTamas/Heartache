using UnityEngine;

namespace Code
{
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SetCameraColliderSize : MonoBehaviour
    {
        private BoxCollider2D _collider;
        private Camera _camera;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            SetSize();
        }

        private void SetSize()
        {
            var height = _camera.orthographicSize * 2.0f;
            var width = height * _camera.aspect;
            _collider.size = new Vector2(width, height);
        }
    }
}
