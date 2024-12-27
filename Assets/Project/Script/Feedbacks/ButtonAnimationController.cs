using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ButtonAnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Scale animation for the button")]
    public Vector3 clickScale = new Vector3(0.9f, 0.9f, 1f);
    public float scaleDuration = 0.2f;

    [Header("Hover Settings")]
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    public float hoverDuration = 0.2f;

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

    public void AnimateHoverEnter()
    {
        // Animate scale on hover enter
        transform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutQuad);
    }

    public void AnimateHoverExit()
    {
        // Animate scale on hover exit
        transform.DOScale(Vector3.one, hoverDuration).SetEase(Ease.OutQuad);
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
            // Instantiate the prefab at the button's position + offset
            Instantiate(feedbackPrefab, transform.position + feedbackOffset, Quaternion.identity);
        }
    }
}
