using Code.Gui;
using Code.Hero;
using Code.Rooms;
using UnityEngine;

namespace Code
{
    public class Exit : MonoBehaviour
    {
        private LevelGenerator _levelGenerator;

        [SerializeField] private GameObject[] _regularExitParts;
        [SerializeField] private GameObject _finalExit;
        [SerializeField] private BoolReference _playerEscaped;

        private bool _isFinalExit;

        public bool FinalExit
        {
            set
            {
                foreach (var regularExitPart in _regularExitParts)
                {
                    regularExitPart.gameObject.SetActive(!value);
                }
                _finalExit.gameObject.SetActive(value);
                _isFinalExit = value;
            }
            get => _isFinalExit;
        }

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
                    if (FinalExit)
                    {
                        _playerEscaped.Variable.Value = true;
                        Countdown.Instance.StopAndClear();
                        MessagePanel.Instance.ShowMessage("You have successfully regained the pieces and escaped!");
                    }
                    else
                    {
                        MessagePanel.Instance.ShowMessage("Level complete!");
                        _levelGenerator.GenerateLevel();
                    }
                }
                else
                {
                    var keysLeft = GlobalProperties.Instance.KeyCountPerLevel - Key.CollectedKeyCount;
                    MessagePanel.Instance.ShowMessage($"There are {keysLeft} fragments left to collect!");
                }
            }
        }
    }
}
