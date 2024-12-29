using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Project.Script.Views
{
    public class TileSpotView : MonoBehaviour
    {
        public event Action<int, int> Clicked;

        [SerializeField] private Button _button;

        private int _x;
        private int _y;

        #region Unity
        private void Awake()
        {
            _button.onClick.AddListener(OnTileClick);
        }
        #endregion

        public Tween AnimatedSetTile(GameObject tile)
        {
            ResetTileState(tile);
            tile.transform.SetParent(transform);
            tile.transform.DOKill();

            return tile.transform.DOMove(transform.position, 0.3f).SetEase(Ease.OutBack);
        }

        public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public void SetTile(GameObject tile)
        {
            tile.transform.SetParent(transform, false);
            tile.transform.position = transform.position;
        }

        private void OnTileClick()
        {
            Clicked?.Invoke(_x, _y);
        }
        
        private void ResetTileState(GameObject tile)
        {
            if (tile != null)
            {
                tile.transform.localRotation = Quaternion.identity;
                tile.transform.localScale = Vector3.one;

                foreach (Transform child in tile.transform)
                {
                    child.localRotation = Quaternion.identity;
                    child.localScale = Vector3.one;
                }
            }
        }
    }
}
