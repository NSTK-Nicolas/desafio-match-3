using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Project.Script.Core;
using Gazeus.DesafioMatch3.Project.Script.Models;
using Gazeus.DesafioMatch3.Project.Script.Views;
using Gazeus.DesafioMatch3.Project.Script.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Project.Script.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private BoardView _boardView;
        [SerializeField] private int _boardHeight = 10;
        [SerializeField] private int _boardWidth = 10;
        [Space]
        [Header("Configurações de Animação do Material")]
        [Space]
        [SerializeField] private GameObject materialGameObject; // GameObject com o Material
        [SerializeField] private float minWaveSpeed = 0.5f; // Velocidade mínima
        [SerializeField] private float maxWaveSpeed = 2.0f; // Velocidade máxima
        [SerializeField] private float speedIncrement = 0.2f; // Incremento por match
        [SerializeField] private float resetDelay = 3.0f; // Tempo de espera antes de resetar a velocidade
        [SerializeField] private float lerpSpeed = 1.0f; // Velocidade do Lerp

        private GameService _gameEngine;
        private bool _isAnimating;
        private int _selectedX = -1;
        private int _selectedY = -1;
        private GameObject _currentlySelectedTile;

        private Material uiMaterial;
        private float currentWaveSpeed;
        private float targetWaveSpeed;
        private float resetTimer;

         private void Awake()
        {
            _gameEngine = new GameService();
            _boardView.TileClicked += OnTileClick;

            // Obtém o material do GameObject com Image
            if (materialGameObject != null)
            {
                Image imageComponent = materialGameObject.GetComponent<Image>();
                if (imageComponent != null)
                {
                    uiMaterial = imageComponent.material;
                }
                else
                {
                    Debug.LogError("O GameObject indicado não possui um componente Image.");
                }
            }

            // Inicializa valores
            currentWaveSpeed = minWaveSpeed;
            targetWaveSpeed = minWaveSpeed;
        }

        private void OnDestroy()
        {
            _boardView.TileClicked -= OnTileClick;
        }

        private void Start()
        {
            List<List<Tile>> board = _gameEngine.StartGame(_boardWidth, _boardHeight);
            _boardView.CreateBoard(board);
        }

        private void Update()
        {
            // Atualiza a velocidade atual com Lerp
            if (currentWaveSpeed != targetWaveSpeed)
            {
                currentWaveSpeed = Mathf.Lerp(currentWaveSpeed, targetWaveSpeed, Time.deltaTime * lerpSpeed);
                UpdateShaderWaveSpeed(currentWaveSpeed);
            }

            // Gerencia o timer para resetar
            if (targetWaveSpeed > minWaveSpeed)
            {
                resetTimer += Time.deltaTime;
                if (resetTimer >= resetDelay)
                {
                    targetWaveSpeed = minWaveSpeed;
                }
            }
        }

        private void AnimateBoard(List<BoardSequence> boardSequences, int index, Action onComplete)
        {
            BoardSequence boardSequence = boardSequences[index];

            Sequence sequence = DOTween.Sequence();

            foreach (var position in boardSequence.MatchedPosition)
            {
                var feedbackController = _boardView.GetFeedbackControllerAtPosition(position.x, position.y);
                if (feedbackController != null)
                {
                    feedbackController.TriggerMatch3Feedback();
                }
            }

            // Atualiza a sequência de matches e ajusta o target da velocidade
            if (boardSequence.MatchedPosition.Count > 0)
            {
                AddToTargetWaveSpeed();
            }

            sequence.Append(_boardView.DestroyTiles(boardSequence.MatchedPosition));
            sequence.Append(_boardView.MoveTiles(boardSequence.MovedTiles));
            sequence.Append(_boardView.CreateTile(boardSequence.AddedTiles));

            index += 1;
            if (index < boardSequences.Count)
            {
                sequence.onComplete += () => AnimateBoard(boardSequences, index, onComplete);
            }
            else
            {
                sequence.onComplete += () => onComplete();
            }
        }

        private void AddToTargetWaveSpeed()
        {
            // Incrementa o target da velocidade
            targetWaveSpeed = Mathf.Clamp(targetWaveSpeed + speedIncrement, minWaveSpeed, maxWaveSpeed);
            resetTimer = 0; // Reseta o timer para evitar o reset imediato
        }

        private void UpdateShaderWaveSpeed(float waveSpeed)
        {
            if (uiMaterial != null)
            {
                uiMaterial.SetFloat("_WaveSpeed", waveSpeed);
            }
        }

        private void OnTileClick(int x, int y)
        {
            if (_isAnimating) return;

            GameObject tile = _boardView.GetTileAtPosition(x, y);
            if (tile != null)
            {
                // Reset the previously selected tile immediately
                if (_currentlySelectedTile != null && _currentlySelectedTile != tile)
                {
                    var previousFeedback = _currentlySelectedTile.GetComponent<TileFeedbackController>();
                    if (previousFeedback != null)
                    {
                        previousFeedback.ResetFeedback();
                    }
                }

                // Update the current selection
                _currentlySelectedTile = tile;

                TileFeedbackController buttonAnimation = tile.GetComponent<TileFeedbackController>();
                if (buttonAnimation != null)
                {
                    buttonAnimation.AnimateSelection();
                }
            }

            if (_selectedX > -1 && _selectedY > -1)
            {
                if (Mathf.Abs(_selectedX - x) + Mathf.Abs(_selectedY - y) > 1)
                {
                    _selectedX = -1;
                    _selectedY = -1;
                }
                else
                {
                    _isAnimating = true;
                    _boardView.SwapTiles(_selectedX, _selectedY, x, y).onComplete += () =>
                    {
                        bool isValid = _gameEngine.IsValidMovement(_selectedX, _selectedY, x, y);
                        if (isValid)
                        {
                            List<BoardSequence> swapResult = _gameEngine.SwapTile(_selectedX, _selectedY, x, y);
                            AnimateBoard(swapResult, 0, () => _isAnimating = false);
                        }
                        else
                        {
                            _boardView.SwapTiles(x, y, _selectedX, _selectedY).onComplete += () => _isAnimating = false;
                        }
                        _selectedX = -1;
                        _selectedY = -1;
                    };
                }
            }
            else
            {
                _selectedX = x;
                _selectedY = y;
            }
        }
    }
}