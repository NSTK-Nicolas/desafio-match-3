using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Project.Script.Core;
using Gazeus.DesafioMatch3.Project.Script.Models;
using Gazeus.DesafioMatch3.Project.Script.Views;
using Gazeus.DesafioMatch3.Project.Script.Feedbacks;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Project.Script.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private BoardView _boardView;
        [SerializeField] private int _boardHeight = 10;
        [SerializeField] private int _boardWidth = 10;

        private GameService _gameEngine;
        private bool _isAnimating;
        private int _selectedX = -1;
        private int _selectedY = -1;

        #region Unity
        private void Awake()
        {
            _gameEngine = new GameService();
            _boardView.TileClicked += OnTileClick;
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
        #endregion

        private void AnimateBoard(List<BoardSequence> boardSequences, int index, Action onComplete)
        {
            BoardSequence boardSequence = boardSequences[index];

            Sequence sequence = DOTween.Sequence();

            // Trigger Match 3 FX before destroying tiles
            foreach (var position in boardSequence.MatchedPosition)
            {
                GameObject tile = _boardView.GetTileAtPosition(position.x, position.y);
                if (tile != null)
                {
                    ButtonAnimationController buttonAnimation = tile.GetComponent<ButtonAnimationController>();
                    if (buttonAnimation != null)
                    {
                        buttonAnimation.TriggerMatch3Feedback();
                    }
                }
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

        private void OnTileClick(int x, int y)
        {
            if (_isAnimating) return;

            GameObject tile = _boardView.GetTileAtPosition(x, y);
            if (tile != null)
            {
                ButtonAnimationController buttonAnimation = tile.GetComponent<ButtonAnimationController>();
                if (buttonAnimation != null)
                {
                    buttonAnimation.AnimateselectedEnter();
                }
            }

            if (_selectedX > -1 && _selectedY > -1)
            {
                if (Mathf.Abs(_selectedX - x) + Mathf.Abs(_selectedY - y) > 1)
                {
                    _selectedX = -1;
                    _selectedY = -1;

                    if (tile != null)
                    {
                        ButtonAnimationController buttonAnimation = tile.GetComponent<ButtonAnimationController>();
                        if (buttonAnimation != null)
                        {
                            buttonAnimation.AnimateSelectedExit();
                        }
                    }
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

                        if (tile != null)
                        {
                            ButtonAnimationController buttonAnimation = tile.GetComponent<ButtonAnimationController>();
                            if (buttonAnimation != null)
                            {
                                buttonAnimation.AnimateSelectedExit();
                            }
                        }
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
