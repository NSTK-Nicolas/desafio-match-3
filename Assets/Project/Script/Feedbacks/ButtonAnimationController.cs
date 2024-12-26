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

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        // Attach default click behavior
        button.onClick.AddListener(() => AnimateClick());
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
}