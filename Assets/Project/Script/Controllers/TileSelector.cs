using Gazeus.DesafioMatch3.Project.Script.Feedbacks;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TileFeedbackController))]
public class TileSelector : MonoBehaviour, IPointerClickHandler
{
    private TileFeedbackController _feedbackController;

    private void Awake()
    {
        _feedbackController = GetComponent<TileFeedbackController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var selectionManager = FindObjectOfType<SelectionManager>();
        if (selectionManager != null)
        {
            selectionManager.SelectTile(_feedbackController);
        }
    }
}