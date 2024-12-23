using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Project.Script.Utils
{
    [ExecuteInEditMode]
    public class CanvasScalerResolutionMatchUpdater : MonoBehaviour
    {
        [SerializeField] private CanvasScaler canvasScaler;
        private const float canvasScalerFactor = 0.65f;
       
        private void OnEnable()
        {
            var screenAspectRatio = GetAspectRatio(Screen.width, Screen.height);

            // if necessary to add a new resolution follow these steps:
            // Add a new loop > insert the aspect ratio > keep loops in descending order

            if (screenAspectRatio >= GetAspectRatio(3, 4) || screenAspectRatio == GetAspectRatio(1668, 2388) || screenAspectRatio == GetAspectRatio(1640, 2360))  // Resultado das divis�es do Aspect Ratio 0,75/0,6985/0,695
            {
                canvasScaler.matchWidthOrHeight = 1f;
            }
          
            else if (screenAspectRatio <= GetAspectRatio(9, 16)) // Resultado da divis�o do Aspect Ratio 0,5625
            {
                canvasScaler.matchWidthOrHeight = 0f;
            }

            else
            {
                canvasScaler.matchWidthOrHeight = canvasScalerFactor; 
            }
        }

        public float GetAspectRatio(float width, float height)
        {
            return (Mathf.Round(width / height * 100)) / 100;
        }

#if UNITY_EDITOR
        [Button("Try Find Scaler in This Object")]
        private void TryFindAndSetScaler()
        {
            var newCanvasScaler = GetComponent<CanvasScaler>();
            if (newCanvasScaler != null)
            {
                canvasScaler = newCanvasScaler;
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}
