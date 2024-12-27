using DG.Tweening;
using Gazeus.DesafioMatch3.Project.Script.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Project.Script.Feedbacks
{
    [RequireComponent(typeof(Button))]
    public class ButtonFeedbackController : MonoBehaviour
    {
        [Header("Animation Settings")]
        [Tooltip("Scale animation for the button")]
        public Vector3 clickScale = new Vector3(0.9f, 0.9f, 1f);
        public float scaleDuration = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("Rotation angle for the hover animation")]
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private float shakeStrength = 1.0f;
        [SerializeField] private int shakeVibrato = 10;

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
        
        public void OnPointerClick(PointerEventData eventData)
        {
            // Notify the SelectionManager of this selection
            var selectionManager = FindObjectOfType<SelectionManager>();
            if (selectionManager != null)
            {
                selectionManager.Select(gameObject);
            }

            // Play selected animation
            AnimateSelectedEnter();
        }


        public void AnimateClick()
        {
            // Animate scale on click
            transform.DOScale(clickScale, scaleDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBounce));
        }

        public void AnimateSelectedEnter()
        {
            // Shake on Z-axis
            transform.DOShakeRotation(shakeDuration, new Vector3(0, 0, shakeStrength), shakeVibrato);
        }

        public void AnimateSelectedExit()
        {
            // Reset rotation on shake exit
            transform.DORotate(Vector3.zero, 0f).SetEase(Ease.Flash);
            foreach (Transform child in transform)
            {
                child.DORotate(Vector3.zero, 0f).SetEase(Ease.Flash);
            }
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
