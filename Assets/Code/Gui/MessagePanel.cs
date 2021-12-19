using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;

namespace Code.Gui
{
    public class MessagePanel : MonoBehaviour
    {
        // TODO: use queue
        public static MessagePanel Instance { get; private set; }

        private const float PanelHideDurationInSeconds = 0.6f;
        
        public float HeaderOffset;
        
        [SerializeField] private TextMeshProUGUI _textGui;
        [SerializeField] private RectTransform _panel;
        
        private TweenerCore<Vector3, Vector3, VectorOptions> _openPanelTweener;
        private TweenerCore<Vector3, Vector3, VectorOptions> _closePanelTweener;
        private float _startY;
        
        private void Awake()
        {
            Instance = this;
            _textGui.text = string.Empty;
            _panel.anchoredPosition = new Vector2(0, -HeaderOffset);
            _startY = _panel.position.y;
        }
        
        public void ShowMessage(string message)
        {
            StopAllCoroutines();
            _openPanelTweener?.Kill();
            _closePanelTweener?.Kill();

            var duration = (0 - _panel.position.y) / (0 - _startY) * PanelHideDurationInSeconds;
            _openPanelTweener = _panel.DOMoveY(0f, duration);
            _openPanelTweener.OnComplete(() =>
            {
                StartAnim(message);
            });
        }

        private void StartAnim(string message)
        {
            var textChars = message.ToCharArray();
            StartCoroutine(AnimateText(textChars));
        }

        private IEnumerator AnimateText(char[] textChars)
        {
            _textGui.text = string.Empty;
            for (var i = 0; i < textChars.Length; i++)
            {
                _textGui.text += textChars[i];
                yield return new WaitForSeconds(0.05f);
            }
            
            yield return new WaitForSeconds(5.0f);

            for (var i = _textGui.text.Length; i >= 0; i--)
            {
                _textGui.text = _textGui.text.Substring(0, i);
                yield return new WaitForSeconds(0.05f);
            }

            _closePanelTweener = _panel.DOMoveY(-HeaderOffset, PanelHideDurationInSeconds);
        }
    }
}
