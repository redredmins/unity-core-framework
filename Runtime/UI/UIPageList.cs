using System;
using UnityEngine;
using UnityEngine.UI;

namespace RedMinS
{
    public class UIPageList : MonoBehaviour
    {
        public delegate void PageHandler();
        PageHandler ChangePage;

        public delegate RectTransform ListItemHandler(int listItemIndex);
        ListItemHandler SetListItem;

        //[SerializeField] Transform listItemParent;    // 리스트아이템 Parent
        public Transform ListItemParent
        {
            get { return transform; }
        }

        [SerializeField] int row = 0;           // 행
        [SerializeField] float[] rowPos;
        [SerializeField] int column = 0;        // 열
        [SerializeField] float[] columePos;

        //[Header("- page")]
        [SerializeField] Text txtPage;

        int allListItem;        // 총 리스트아이템 갯수
        int listItemPerPage;    // 한 페이지의 리스트아이템 수
        int curPage = 0;    // 지금 페이지 인덱스
        int lastPage = 0;   // 마지막 페이지 인덱스
        int maxPage = 0;    // 총 페이지 수


        public void InitList(int allListItem, PageHandler pageHandler, ListItemHandler listItemHandler)
        {
            this.allListItem = allListItem;
            listItemPerPage = row * column;

            curPage = 0;
            maxPage = (int)Math.Ceiling((decimal)allListItem / (decimal)listItemPerPage);
            lastPage = maxPage - 1;
            ChangePage = pageHandler;

            SetListItem = listItemHandler;
            SetPage(curPage);
        }

        void SetPage(int page)
        {
            curPage = page;
            int curRow = 0;
            int curColumn = 0;

            ChangePage();

            int firstIndex = page * listItemPerPage;
            int firstIndexInNextPage = (page + 1) * listItemPerPage;
            for (int i = firstIndex; i < firstIndexInNextPage; ++i)
            {
                if (i < allListItem)
                {
                    RectTransform rect = SetListItem(i);
                    //rect.SetParent(listObj.transform);
                    rect.localEulerAngles = new Vector3(0f, 0f, 0f);
                    rect.localScale = new Vector3(1f, 1f, 1f);
                    rect.localPosition = new Vector3(rowPos[curRow], columePos[curColumn], 0f);

                    curRow += 1;
                    if (curRow >= rowPos.Length)
                    {
                        curRow = 0;
                        curColumn += 1;
                    }
                }
            }

            txtPage.text = string.Format("{0} / {1}", curPage + 1, maxPage);
        }

        public void ClickPreviousPage()
        {
            int newPage = curPage - 1;
            if (newPage < 0)
            {
                newPage = lastPage;
            }

            SetPage(newPage);
        }

        public void ClickNextPage()
        {
            int newPage = curPage + 1;
            if (newPage > lastPage)
            {
                newPage = 0;
            }

            SetPage(newPage);
        }

    }
}