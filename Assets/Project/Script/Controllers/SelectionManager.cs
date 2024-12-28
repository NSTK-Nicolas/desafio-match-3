using DG.Tweening;
using Gazeus.DesafioMatch3.Project.Script.Feedbacks;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    private TileFeedbackController _currentlySelectedTile;
    /// <summary>
    /// Seleciona um novo tile e reseta o anterior, se necessário.
    /// </summary>
    /// <param name="newTile">O novo tile a ser selecionado.</param>
    public void SelectTile(TileFeedbackController newTile)
    {
        // Reseta o feedback do tile atualmente selecionado, se for diferente do novo
        if (_currentlySelectedTile != null && _currentlySelectedTile != newTile)
        {
            _currentlySelectedTile.ResetFeedback();
            Debug.Log("Resetei tudo");
        }

        // Atualiza o tile selecionado e anima
        _currentlySelectedTile = newTile;
        _currentlySelectedTile.AnimateSelection();
    }
    
}