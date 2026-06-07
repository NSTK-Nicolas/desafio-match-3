using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Project.Script.Feedbacks;
using Gazeus.DesafioMatch3.Project.Script.Models;
using Gazeus.DesafioMatch3.Project.Script.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Project.Script.Views
{
    public class BoardView : MonoBehaviour
    {
        public event Action<int, int> TileClicked;

        [SerializeField] private GridLayoutGroup _boardContainer;
        [SerializeField] private TilePrefabRepository _tilePrefabRepository;
        [SerializeField] private TileSpotView _tileSpotPrefab;
        
        private GameObject[][] _tiles;
        private TileSpotView[][] _tileSpots;
        
        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve spawnMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve spawnScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float spawnDuration = 0.5f;

        [SerializeField] private AnimationCurve destroyScaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [SerializeField] private float destroyDuration = 0.35f;

        public void CreateBoard(List<List<Tile>> board)
        {
            _boardContainer.constraintCount = board[0].Count;
            _tiles = new GameObject[board.Count][];
            _tileSpots = new TileSpotView[board.Count][];

            for (int y = 0; y < board.Count; y++)
            {
                _tiles[y] = new GameObject[board[0].Count];
                _tileSpots[y] = new TileSpotView[board[0].Count];

                for (int x = 0; x < board[0].Count; x++)
                {
                    TileSpotView tileSpot = Instantiate(_tileSpotPrefab);
                    tileSpot.transform.SetParent(_boardContainer.transform, false);
                    tileSpot.SetPosition(x, y);
                    tileSpot.Clicked += TileSpot_Clicked;

                    _tileSpots[y][x] = tileSpot;

                    int tileTypeIndex = board[y][x].Type;
                    if (tileTypeIndex > -1)
                    {
                        GameObject tilePrefab = _tilePrefabRepository.TileTypePrefabList[tileTypeIndex];
                        GameObject tile = Instantiate(tilePrefab);
                        tileSpot.SetTile(tile);

                        _tiles[y][x] = tile;
                    }
                }
            }
        }
        
        public TileFeedbackController GetFeedbackControllerAtPosition(int x, int y)
        {
            if (y >= 0 && y < _tileSpots.Length && x >= 0 && x < _tileSpots[y].Length)
            {
                TileSpotView tileSpot = _tileSpots[y][x];
                if (tileSpot != null)
                {
                    return tileSpot.GetComponent<TileFeedbackController>();
                }
            }
            return null;
        }
        
        public GameObject GetTileAtPosition(int x, int y)
        {
            if (y >= 0 && y < _tiles.Length && x >= 0 && x < _tiles[y].Length)
            {
                return _tiles[y][x];
            }
            return null;
        }

        public Tween CreateTile(List<AddedTileInfo> addedTiles)
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < addedTiles.Count; i++)
            {
                AddedTileInfo addedTileInfo = addedTiles[i];
                Vector2Int position = addedTileInfo.Position;

                TileSpotView tileSpot = _tileSpots[position.y][position.x];

                GameObject tilePrefab = _tilePrefabRepository.TileTypePrefabList[addedTileInfo.Type];
                GameObject tile = Instantiate(tilePrefab);
                tileSpot.SetTile(tile);

                _tiles[position.y][position.x] = tile;

                tile.transform.localScale = Vector2.zero;

                // Set initial position above the grid for falling effect
                sequence.Join(tile.transform.DOMove(tileSpot.transform.position, spawnDuration)
                    .SetEase(spawnMoveCurve)
                    .SetDelay(0.05f));

                sequence.Join(tile.transform.DOScale(1.0f, spawnDuration)
                    .SetEase(spawnScaleCurve)
                    .SetDelay(0.05f));
            }

            return sequence;
        }

        public Tween DestroyTiles(List<Vector2Int> matchedPosition)
        {
            Sequence sequence = DOTween.Sequence();
    
            for (int i = 0; i < matchedPosition.Count; i++)
            {
                Vector2Int position = matchedPosition[i];
                GameObject tile = _tiles[position.y][position.x];
        
                // Anima a escala para 0 antes de destruir usando a curva
                sequence.Join(tile.transform.DOScale(Vector3.zero, destroyDuration)
                    .SetEase(destroyScaleCurve)
                    .OnComplete(() => Destroy(tile)));
            
                _tiles[position.y][position.x] = null;
            }

            return sequence;
        }

        public Tween MoveTiles(List<MovedTileInfo> movedTiles)
        {
            GameObject[][] tiles = new GameObject[_tiles.Length][];
            for (int y = 0; y < _tiles.Length; y++)
            {
                tiles[y] = new GameObject[_tiles[y].Length];
                for (int x = 0; x < _tiles[y].Length; x++)
                {
                    tiles[y][x] = _tiles[y][x];
                }
            }

            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < movedTiles.Count; i++)
            {
                MovedTileInfo movedTileInfo = movedTiles[i];

                Vector2Int from = movedTileInfo.From;
                Vector2Int to = movedTileInfo.To;

                sequence.Join(_tileSpots[to.y][to.x].AnimatedSetTile(_tiles[from.y][from.x]));

                tiles[to.y][to.x] = _tiles[from.y][from.x];
            }

            _tiles = tiles;

            return sequence;
        }

        public Tween SwapTiles(int fromX, int fromY, int toX, int toY)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_tileSpots[fromY][fromX].AnimatedSetTile(_tiles[toY][toX]));
            sequence.Join(_tileSpots[toY][toX].AnimatedSetTile(_tiles[fromY][fromX]));

            (_tiles[toY][toX], _tiles[fromY][fromX]) = (_tiles[fromY][fromX], _tiles[toY][toX]);

            // Reset rotation and scale of the tiles
            ResetTileTransform(_tiles[toX][toY]);
            ResetTileTransform(_tiles[fromX][fromY]);

            return sequence;
        }
        
        private void ResetTileTransform(GameObject tile)
        {
            if (tile != null)
            {
                var feedbackController = tile.GetComponent<TileFeedbackController>();
                if (feedbackController != null)
                {
                    feedbackController.ResetFeedback();
                }
            }
        }

        #region Events
        private void TileSpot_Clicked(int x, int y)
        {
            TileClicked(x, y);
        }
        #endregion
    }
}
