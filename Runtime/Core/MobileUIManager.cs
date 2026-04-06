using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace RedMinS
{
    public enum BaseScreenRatio
    {
        HD, // 720 x 1280
        FullHD, // 1080 x 1920
        MobileHD, // 720 x 1600
        MobileFHD, // 1080 x 2400
    }

    public class MobileUIManager : UIManager
    {
        // [SerializeField] TextMeshProUGUI txtLoadingMessage;
        // [SerializeField] GameObject efcClick;

        [SerializeField] BaseScreenRatio baseScreenRatio;
        Vector2 MOBILE_RATIO_SCREEN
        {
            get
            {
                switch (baseScreenRatio)
                {
                    default:
                    case BaseScreenRatio.HD: return new Vector2(720f, 1280f);
                    case BaseScreenRatio.FullHD: return new Vector2(1080f, 1920f);
                    case BaseScreenRatio.MobileHD: return new Vector2(720f, 1600f);
                    case BaseScreenRatio.MobileFHD: return new Vector2(1080f, 2400f);
                }
            }
        }

        public bool IsPopupActive => _activePopups.Count > 0;


        protected void Start()
        {
            //SupportResolution(canvas);

        //#if (!UNITY_EDITOR) && (UNITY_ANDROID || UNITY_IPHONE)
            //EventSystem.current.pixelDragThreshold = (int)(0.5f * Screen.dpi / 2.54f); // 0.5cm
        //#endif
        }

        public void OnBackButtonPressed()
        {
            if (IsPopupActive)
            {
                int last = _activePopups.Count - 1;
                _activePopups[last].OnBackKeyAction();
            }
            else
            {
                AskExitGame();
            }
        }
        

        /*
        public void SupportResolution(Canvas canv, GameObject gameSceneObj = null)
        {
            CanvasScaler cs = canv.GetComponent<CanvasScaler>();

        #if UNITY_ANDROID || UNITY_IPHONE
            float ratioChk = canv.pixelRect.width / canv.pixelRect.height;
            if (ratioChk <= 0.5f) // 모바일 세로가 긴 해상도라면
            {
            #if UNITY_ANDROID
                cs.referenceResolution = MOBILE_RATIO_SCREEN;
                cs.matchWidthOrHeight = 1f;
                if (gameSceneObj != null)
                {
                    gameSceneObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                }
            #elif UNITY_IPHONE
                cs.referenceResolution = MOBILE_RATIO_SCREEN;
                cs.matchWidthOrHeight = 1f;
                if (gameSceneObj != null)
                {
                    gameSceneObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                }
            #endif
            }
            else // 모바일 일반 해상도
            {
                cs.referenceResolution = MOBILE_RATIO_SCREEN;
                cs.matchWidthOrHeight = 1f;
            }
        #endif
        }
        */

        void AskExitGame()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            var uiString = Core.app.table.uiString;
            string message = uiString.GetString(9013);
            string yesTxt = uiString.GetString(1003);
            string noTxt = uiString.GetString(1004);
            ShowSystemPopup(true, message, yesTxt, Application.Quit, noTxt, null);
#endif
        }

    }
}