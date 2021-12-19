using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class RandomObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private List<Sprite> _sprites;

        private void Awake()
        {
            var randomSprite = _sprites.GetRandomElement();
            _renderer.sprite = randomSprite;
        }
    }
}
