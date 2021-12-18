using System;
using Code.Gui;
using Code.Rooms;
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
                if (Key.KeysAreCollected)
                {
                    MessagePanel.Instance.ShowMessage("Level complete!");
                    _levelGenerator.GenerateLevel();
                }
                else
                {
                    var keysLeft = GlobalProperties.Instance.KeyCount - Key.CollectedKeyCount;
                    MessagePanel.Instance.ShowMessage($"There are {keysLeft} fragments left to collect!");
                }
            }
        }
    }
}
