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
        [SerializeField] private GameObject materialGameObject;
        [SerializeField] private float minWaveSpeed = 0.5f;
        [SerializeField] private float maxWaveSpeed = 2.0f;
        [SerializeField] private float speedIncrement = 0.2f;
        [SerializeField] private float resetDelay = 3.0f;
        [SerializeField] private float lerpSpeed = 1.0f;

        private GameService _gameEngine;
        private bool _isAnimating;
        private int _selectedX = -1;
        private int _selectedY = -1;
        private GameObject _currentlySelectedTile;

        private Material uiMaterial;
        private float currentWaveSpeed;
        private float targetWaveSpeed;
        private float resetTimer;

        // Nova variável: controla se a cascata de matches está ativa
        private bool _isCascadeActive = false;

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
            // Atualiza a velocidade atual com Lerp para suavidade
            if (Mathf.Abs(currentWaveSpeed - targetWaveSpeed) > 0.001f)
            {
                currentWaveSpeed = Mathf.Lerp(currentWaveSpeed, targetWaveSpeed, Time.deltaTime * lerpSpeed);
                UpdateShaderWaveSpeed(currentWaveSpeed);
            }

            // Só inicia o timer de reset se a cascata NÃO estiver mais ativa
            // Isso garante que o shader continue acumulando ondulações durante toda a sequência
            if (!_isCascadeActive && targetWaveSpeed > minWaveSpeed)
            {
                resetTimer += Time.deltaTime;
                if (resetTimer >= resetDelay)
                {
                    // O target volta ao mínimo e o Lerp acima suaviza a transição
                    targetWaveSpeed = minWaveSpeed;
                }
            }
        }

        private void AnimateBoard(List<BoardSequence> boardSequences, int index, Action onComplete)
        {
            BoardSequence boardSequence = boardSequences[index];

            Sequence sequence = DOTween.Sequence();

            // Quantidade de peças destruídas nesta etapa da cascata
            int matchSize = boardSequence.MatchedPosition.Count;

            foreach (var position in boardSequence.MatchedPosition)
            {
                var feedbackController = _boardView.GetFeedbackControllerAtPosition(position.x, position.y);
                if (feedbackController != null)
                {
                    // Passa o tamanho do match para permitir feedbacks distintos
                    feedbackController.TriggerMatchFeedback(matchSize);
                }
            }

            // Atualiza a sequência de matches e ajusta o target da velocidade do shader
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
            // Incrementa o target da velocidade progressivamente durante a cascata
            targetWaveSpeed = Mathf.Clamp(targetWaveSpeed + speedIncrement, minWaveSpeed, maxWaveSpeed);
            // Não reseta o timer aqui pois a cascata está ativa e o timer está pausado
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
                            _isCascadeActive = true;

                            List<BoardSequence> swapResult = _gameEngine.SwapTile(_selectedX, _selectedY, x, y);
                            AnimateBoard(swapResult, 0, () =>
                            {
                                _isAnimating = false;
                                _isCascadeActive = false;
                                resetTimer = 0f;
                            });
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
