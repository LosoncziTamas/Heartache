using System.Collections;
using Code.Hero;
using TMPro;
using UnityEngine;

namespace Code.Gui
{
    public class Countdown : MonoBehaviour
    {
        public static Countdown Instance { get; private set; }
        
        [SerializeField] private TextMeshProUGUI _countDownText;

        private void Awake()
        {
            Instance = this;
            _countDownText.text = string.Empty;
        }

        public void StartCountDown(float totalTime)
        {
            StopAllCoroutines();
            StartCoroutine(CountdownTick(totalTime));
        }

        private IEnumerator CountdownTick(float totalTime)
        {
            var timeElapsed = 0f;
            while (timeElapsed < totalTime)
            {
                timeElapsed += Time.deltaTime;
                var countDown = Mathf.RoundToInt(totalTime - timeElapsed);
                _countDownText.text = $"Time left: {countDown:D}";
                yield return null;
            }
            StopAndClear();
            HeroController.Instance.Die();
        }

        public void StopAndClear()
        {
            StopAllCoroutines();
            _countDownText.text = string.Empty;
        }
    }
}
