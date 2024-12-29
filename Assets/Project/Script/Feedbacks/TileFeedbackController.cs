using DG.Tweening;
using Gazeus.DesafioMatch3.Project.Script.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Project.Script.Feedbacks
{
    [RequireComponent(typeof(Button))]
    public class TileFeedbackController : MonoBehaviour
    {
        [Header("Animation Settings")]
        [Tooltip("Scale animation for the button")]
        public Vector3 clickScale = new Vector3(0.9f, 0.9f, 1f);
        public float scaleDuration = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("Rotation angle for the hover animation")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private float trembleDuration = 0.2f;
        [SerializeField] private float trembleAngle = 15f;
        [SerializeField] private int trembleLoops = 3;
        
        private Tween _currentTween;

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

            button.onClick.AddListener(() => {
                AnimateClick();
                InstantiateFeedback(clickFeedbackPrefab);
            });
        }
        

        public void AnimateClick()
        {
            transform.DOScale(clickScale, scaleDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBounce));
        }

        public void AnimateSelection()
        {
            // Garante que qualquer animação anterior seja interrompida
            _currentTween?.Kill();

            // Anima a rotação no eixo Z para um efeito de tremelique
            _currentTween = visualTransform.DORotate(new Vector3(0, 0, trembleAngle), trembleDuration / 2)
                .SetLoops(trembleLoops * 2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine).OnComplete(ResetFeedback);

        }

        /// <summary>
        /// Reseta a rotação do GameObject para zero.
        /// </summary>
        public void ResetFeedback()
        {
            // Reseta a rotação e escala do visualTransform
            visualTransform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.OutQuad);
            visualTransform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);

            // Reseta a rotação de todos os filhos no eixo Z
            foreach (Transform child in visualTransform)
            {
                child.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }


        public void TriggerMatch3Feedback()
        {
            InstantiateFeedback(match3FeedbackPrefab);
        }

        private void InstantiateFeedback(GameObject feedbackPrefab)
        {
            if (feedbackPrefab != null)
            {
                GameObject feedbackInstance = Instantiate(feedbackPrefab, transform.position + feedbackOffset, Quaternion.identity);
                feedbackInstance.transform.SetParent(transform, true);
            }
        }

        
    }
}
