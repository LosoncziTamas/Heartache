using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.Gui
{
    public class MessagePanel : MonoBehaviour
    {
        public static MessagePanel Instance { get; private set; }
        
        [SerializeField] private TextMeshProUGUI _textGui;
        [SerializeField] private RectTransform _panel;

        public float HeaderOffset;
        
        private bool _hidden;
        
        private void Awake()
        {
            Instance = this;
            _textGui.text = string.Empty;
            _panel.anchoredPosition = new Vector2(0, -HeaderOffset);
            _hidden = true;
        }

        public void ShowMessage(string message)
        {
            StopAllCoroutines();
            if (_hidden)
            {
                _panel.DOMoveY(0f, 0.6f).OnComplete(() =>
                {
                    _hidden = false;
                    StartAnim(message);
                });
            }
            else
            {
                StartAnim(message);
            }

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
            
            _panel.DOMoveY(-HeaderOffset, 0.6f).OnComplete(() =>
            {
                _hidden = true;
            });
        }
    }
}
