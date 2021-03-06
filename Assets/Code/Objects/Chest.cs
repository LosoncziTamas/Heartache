using System.Collections;
using Code.Common;
using Code.Gui;
using DG.Tweening;
using UnityEngine;

namespace Code.Objects
{
    public class Chest : MonoBehaviour
    {
        public static bool KeysAreCollected => CollectedChestCount == GlobalProperties.Instance.KeyCountPerLevel;
        private static readonly int Open = Animator.StringToHash("Open");
        public static int CollectedChestCount { get; set; }
        
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _heart;
        public bool Collected { get; private set; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player) && !Collected)
            {
                OnKeyCollected();
            }
        }

        private IEnumerator HideHeart()
        {
            yield return new WaitForSeconds(1.0f);
            _heart.transform.DOPunchScale(Vector3.one * 0.6f, 0.7f).OnComplete(() =>
            {
                _heart.gameObject.SetActive(false);
            });
        }

        private void OnKeyCollected()
        {
            Collected = true;
            _animator.SetBool(Open, true);
            StartCoroutine(HideHeart());
            CollectedChestCount++;
            if (KeysAreCollected)
            {
                // TODO: differentiate completion
                MessagePanel.Instance.ShowMessage("All fragments collected! Hurry up and move on to the next level.");
            }
            else
            {
                MessagePanel.Instance.ShowMessage( $"A fragment collected. {GlobalProperties.Instance.KeyCountPerLevel - CollectedChestCount} more to go!");
            }
        }
    }
}