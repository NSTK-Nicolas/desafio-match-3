using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Project.Script.Feedbacks
{
    [RequireComponent(typeof(Button))]
    public class ButtonAnimationController : MonoBehaviour
    {
        [Header("Animation Settings")]
        [Tooltip("Scale animation for the button")]
        public Vector3 clickScale = new Vector3(0.9f, 0.9f, 1f);
        public float scaleDuration = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("Rotation angle for the hover animation")]
        public Vector3 selectedRotation = new Vector3(0f, 0f, 15f);
        public float rotationDuration = 0.2f;

        [Header("Feedback Settings")]
        [Tooltip("Prefab to instantiate on click")]
        public GameObject clickFeedbackPrefab;
        [Tooltip("Prefab to instantiate on Match 3")]
        public GameObject match3FeedbackPrefab;
    
        [Tooltip("Position offset for the feedback instantiation")]
        public Vector3 feedbackOffset = Vector3.zero;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            // Attach default click behavior
            button.onClick.AddListener(() => {
                AnimateClick();
                InstantiateFeedback(clickFeedbackPrefab);
            });
        }

        public void AnimateClick()
        {
            // Animate scale on click
            transform.DOScale(clickScale, scaleDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBounce));
        }

        public void AnimateselectedEnter()
        {
            // Animate rotation on hover enter
            transform.DORotate(selectedRotation, rotationDuration).SetEase(Ease.OutQuad);
        }

        public void AnimateSelectedExit()
        {
            // Reset rotation on hover exit
            transform.DORotate(Vector3.zero, rotationDuration).SetEase(Ease.OutQuad);
        }

        public void TriggerMatch3Feedback()
        {
            // Instantiate feedback for Match 3
            InstantiateFeedback(match3FeedbackPrefab);
        }

        private void InstantiateFeedback(GameObject feedbackPrefab)
        {
            if (feedbackPrefab != null)
            {
                // Instantiate the prefab as a child of this GameObject
                GameObject feedbackInstance = Instantiate(feedbackPrefab, transform.position + feedbackOffset, Quaternion.identity);
                feedbackInstance.transform.SetParent(transform, true);
            }
        }
    }
}
