using System;
using UnityEngine;

namespace Code
{
    public class Exit : MonoBehaviour
    {
        private LevelGenerator _levelGenerator;
        
        private void Awake()
        {
            _levelGenerator = GetComponentInParent<LevelGenerator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player))
            {
                if (Key.CollectedKeyCount == GlobalProperties.Instance.KeyCount)
                {
                    Debug.Log("Level Complete");
                    _levelGenerator.GenerateLevel();
                }
                else
                {
                    var keysLeft = GlobalProperties.Instance.KeyCount - Key.CollectedKeyCount;
                    Debug.Log($"There are {keysLeft} keys left to collect!");
                }
            }
        }
    }
}
