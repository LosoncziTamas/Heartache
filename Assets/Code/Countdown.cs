using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Code
{
    public class Countdown : MonoBehaviour
    {
        public static Countdown Instance { get; private set; }
        
        [SerializeField] private TextMeshProUGUI _countDownText;

        private void Awake()
        {
            Instance = this;
        }

        public void StartCountDown(float totalTime)
        {
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
        }
    }
}
