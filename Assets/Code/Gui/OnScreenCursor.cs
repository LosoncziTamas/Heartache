using Code.Hero;
using UnityEngine;

namespace Code.Gui
{
    // TODO: remove
    public class OnScreenCursor : MonoBehaviour
    {
        [SerializeField] private LineRenderer _renderer;
        [SerializeField] private FloatReference _range;
        
        private Camera _camera;
        private Transform _hero;

        private void Start()
        {
            _camera = Camera.main;
            _hero = HeroController.Instance.transform;
        }

        public void Update()
        {
            var mousePos = (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition);
            var heroPos = (Vector2)_hero.position;
            var targetPos = heroPos + (mousePos - heroPos).normalized * _range.Value;
            _renderer.SetPosition(0, heroPos);
            _renderer.SetPosition(1, targetPos);
        }
    }
}