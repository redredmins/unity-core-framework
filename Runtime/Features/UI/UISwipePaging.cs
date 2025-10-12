using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

namespace RedMinS.UI
{
    //[Serializable]
    public class UISwipePaging : MonoBehaviour
    {
        [SerializeField] RectTransform pagesRect;
        [SerializeField] float lengthToSwipe = 100f;

        [Header("- button")]
        [SerializeField] Button btnPrev;
        [SerializeField] Button btnNext;

        [Header("- optional")]
        [SerializeField] UIDotIndicator dotIndicator;

        UnityAction<int> _onShowPageAction;
        int _curIndex = 0;
        int _allPage = 0;


        public void InitSwipePaging(int allPage, UnityAction<int> onShowPage, int startIndex = 0)
        {
            _allPage = allPage;
            _onShowPageAction = onShowPage;

            //pagesRect.localPosition = Vector3.zero;
            _curIndex = startIndex;
            SetPage(_curIndex);

            btnPrev.onClick.RemoveAllListeners();
            btnPrev.onClick.AddListener(ShowPrevPage);
            btnNext.onClick.RemoveAllListeners();
            btnNext.onClick.AddListener(ShowNextPage);
        }

        void SetPage(int index)
        {
            _curIndex = index;
            btnPrev.gameObject.SetActive(index > 0);
            btnNext.gameObject.SetActive(index < (_allPage -1));

            float pagesXpos = (lengthToSwipe * index) * -1f;
            pagesRect.DOLocalMoveX(pagesXpos, 0.5f);

            if (_onShowPageAction != null) _onShowPageAction(index);
            if (dotIndicator != null) dotIndicator.SetIndicator(index);
        }

        void ShowPrevPage()
        {
            if (_curIndex > 0)
            {
                _curIndex -= 1;
                SetPage(_curIndex);
            }
        }

        void ShowNextPage()
        {
            if (_curIndex < (_allPage -1))
            {
                _curIndex += 1;
                SetPage(_curIndex);
            }
        }

    }
}