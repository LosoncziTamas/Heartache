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
            _countDownText.text = string.Empty;
            HeroController.Instance.Die();
        }
        
        private void OnGUI()
        {
            GUILayout.Space(200);
            if (GUILayout.Button("Die"))
            {
                StopAllCoroutines();
                _countDownText.text = string.Empty;
                HeroController.Instance.Die();
            }
        }
    }
}
