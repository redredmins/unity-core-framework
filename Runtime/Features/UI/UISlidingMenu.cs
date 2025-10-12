using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace RedMinS.UI
{
    public class UISlidingMenu : MonoBehaviour
    {
        public enum SlidingDirection
        {
            Vertical, Horizontal
        }

        [SerializeField] protected SlidingDirection slidingDirection = SlidingDirection.Vertical;

        [Header("- menu")]
        [SerializeField] protected RectTransform menuRect;
        [SerializeField] protected RectTransform menuHeaderRect;

        [Tooltip("- 0: up / 1: down")]
        [SerializeField] protected float[] yPoses;
        [SerializeField] protected Vector2[] headerSizes;

        public bool isDown { protected set; get; }

        SoundManager _sound;


        protected void Awake()
        {
            isDown = true;

            _sound = Core.app.sound;
        }

        public virtual void ClickMenuUp()
        {
            if (!isDown) return;
            //Debug.Log("ClickMenuUp");
            isDown = false;

            if (slidingDirection == SlidingDirection.Vertical)
            {
                menuRect.DOAnchorPosY(yPoses[0], 0.5f); //(0f, 0.5f);
                if(menuHeaderRect != null) menuHeaderRect.DOSizeDelta(headerSizes[1], 0.5f); //new Vector2((760f, 100f), 0.5f);
            }
            else
            {
                menuRect.DOAnchorPosX(yPoses[0], 0.5f); //(0f, 0.5f);
                if(menuHeaderRect != null) menuHeaderRect.DOSizeDelta(headerSizes[1], 0.5f);
            }
        }

        public virtual void ClickMenuDown()
        {
            if (isDown) return;
            //Debug.Log("ClickMenuDown");
            isDown = true;

            if (slidingDirection == SlidingDirection.Vertical)
            {
                menuRect.DOAnchorPosY(yPoses[1], 0.5f); //(-730f, 0.5f);
                if(menuHeaderRect != null) menuHeaderRect.DOSizeDelta(headerSizes[0], 0.5f); //new Vector2(760f, 200f), 0.5f);
            }
            else
            {
                menuRect.DOAnchorPosX(yPoses[1], 0.5f); //(-730f, 0.5f);
                if(menuHeaderRect != null) menuHeaderRect.DOSizeDelta(headerSizes[0], 0.5f);
            }
        }

    }
}