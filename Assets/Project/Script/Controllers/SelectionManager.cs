using Gazeus.DesafioMatch3.Project.Script.Feedbacks;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Project.Script.Controllers
{
    public class SelectionManager : MonoBehaviour
    {
        private GameObject _currentlySelected;

        public void Select(GameObject newSelection)
        {
            // Reset the previous selection
            if (_currentlySelected != null && _currentlySelected != newSelection)
            {
                var feedbackController = _currentlySelected.GetComponent<ButtonFeedbackController>();
                if (feedbackController != null)
                {
                    feedbackController.AnimateSelectedExit();
                }
            }

            // Update the currently selected object
            _currentlySelected = newSelection;
        }

        public void DeselectCurrent()
        {
            if (_currentlySelected != null)
            {
                var feedbackController = _currentlySelected.GetComponent<ButtonFeedbackController>();
                if (feedbackController != null)
                {
                    feedbackController.AnimateSelectedExit();
                }

                _currentlySelected = null;
            }
        }
    }
}