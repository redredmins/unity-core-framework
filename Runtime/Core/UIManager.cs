using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace RedMinS
{
    [Serializable]
    public abstract class UIManager : SingletonMonobehaviour<UIManager>
    {
        [SerializeField] protected Canvas canvas;

        [Header("- UI Effects")]
        [SerializeField] protected Animator fadeAni;
        [SerializeField] protected Image fadeBoard;
        [SerializeField] protected Animation loadingSpinner;

        
        [Header("- UI Prefabs")]
        [SerializeField] protected GameObject uiSystemPopup;
        [SerializeField] protected GameObject uiToast;

        // 
        protected List<UIPopup> _activePopups = null;
        protected List<string> _systemMessageList = null;
        protected ObjectPool _pool;



        protected override void OnSingletonAwake()
        {
            _activePopups = new List<UIPopup>();
            _systemMessageList = new List<string>();
            _pool = new ObjectPool();
        }


        // 팝업
        public virtual void OnPopup(UIPopup popup)
        {
            _activePopups.Add(popup);

            Core.app.sound.PlayEffectSound("Popup On");
        }

        public virtual void OffPopup(UIPopup popup)
        {
            if (_activePopups.Contains(popup))
            {
                _activePopups.Remove(popup);

                Core.app.sound.PlayEffectSound("Popup Off");
            }
        }

        
        // 시스템 경고 & 강제종료
        public void ShowSystemPopupAndQuit(string message, UnityAction action = null)
        {
            Core.app.sound.PlayEffectSound("Warning");

            Core.network.StopAllCoroutines();
            //Core.network.assetLoader.StopAllCoroutines();
            Core.database.StopAllCoroutines();

            ShowSystemPopup(false, message,
                () => {
                    if (action != null) action();
                    Application.Quit();
                });
        }

        // 시스템 팝업
        protected void ShowSystemPopup(bool isSmallError, SystemPopupMessage systemMessage)
        {
            // var sound = Core.app.sound;
            // if (isSmallError) sound.PlayEffectSound(sound.soundClips.acWarning);
            // else sound.PlayEffectSound(Core.app.sound.soundClips.acPopupOn);

            GameObject systemPopupObj = _pool.CreateObject(uiSystemPopup, canvas.transform);
            UISystemPopup systemPopup = systemPopupObj.GetComponent<UISystemPopup>();
            systemPopup.SetSystemPopup(systemMessage, RemoveSystemPopup);
        }

        public void ShowSystemPopup(bool isSmallError, string message, UnityAction okBtnAction)
        {
            if (_systemMessageList.Contains(message) == false)
            {
                _systemMessageList.Add(message);

                var uiString = Core.app.table.uiString;
                SystemPopupMessage systemMessage = new SystemPopupMessage();
                systemMessage.message = message;
                systemMessage.yes = uiString.GetString(1001);
                systemMessage.yesBtnAction = okBtnAction;

                ShowSystemPopup(isSmallError, systemMessage);
            }
        }

        public void ShowSystemPopup(bool isSmallError, string message, string yes, UnityAction yesBtnAction)
        {
            if (_systemMessageList.Contains(message) == false)
            {
                _systemMessageList.Add(message);

                SystemPopupMessage systemMessage = new SystemPopupMessage();
                systemMessage.message = message;
                systemMessage.yes = yes;
                systemMessage.yesBtnAction = yesBtnAction;

                ShowSystemPopup(isSmallError, systemMessage);
            }
        }

        public void ShowSystemPopup(bool isSmallError, string message,
                                string yes, UnityAction yesBtnAction,
                                string no, UnityAction noBtnAction)
        {
            if (_systemMessageList.Contains(message) == false)
            {
                _systemMessageList.Add(message);

                SystemPopupMessage systemMessage = new SystemPopupMessage();
                systemMessage.message = message;
                systemMessage.yes = yes;
                systemMessage.yesBtnAction = yesBtnAction;
                systemMessage.no = no;
                systemMessage.noBtnAction = noBtnAction;

                ShowSystemPopup(isSmallError, systemMessage);
            }
        }

        public void ShowSystemPopup(bool isSmallError, int message,
                                int yes, UnityAction yesBtnAction,
                                int no, UnityAction noBtnAction)
        {
            var uiString = Core.app.table.uiString;
            ShowSystemPopup(isSmallError, uiString.GetString(message),
                            uiString.GetString(yes), yesBtnAction,
                            uiString.GetString(no), noBtnAction);
        }

        protected void RemoveSystemPopup(string message, GameObject popup)
        {
            _systemMessageList.Remove(message);
            _pool.RemoveObject(popup);
        }


        /// 토스트
        protected Queue<string> _toastMessageQueue = new Queue<string>();
        public void ShowToast(string message)
        {
            //AnalyticsManager.DebugLog("toast M => " + message);
            if (_toastMessageQueue.Contains(message) == false)
            {
                _toastMessageQueue.Enqueue(message);

                if (_toastMessageQueue.Count == 1)
                {
                    StartCoroutine(IEToast());
                }
            }
        }

        public void ShowToast(int uid)
        {
            string message = Core.app.table.uiString.GetString(uid);
            ShowToast(message);
        }

        protected IEnumerator IEToast()
        {
            while (_toastMessageQueue.Count > 0)
            {
                // var sound = Core.app.sound;
                // sound.PlayEffectSound(sound.soundClips.acToast);
                GameObject toastObj = _pool.CreateObject(uiToast, canvas.transform);
                UIToast toast = toastObj.GetComponent<UIToast>();
                toast.SetToast(_toastMessageQueue.Peek());

                yield return new WaitForSeconds(1.5f);

                _toastMessageQueue.Dequeue();
                _pool.RemoveObject(toastObj);
            }
        }

        /// 페이드 인 효과
        protected WaitForSeconds halfSec = new WaitForSeconds(0.5f);
        public IEnumerator IEFadeIn()
        {
            fadeAni.SetTrigger("FADE_IN");
            yield return halfSec;
        }

        public IEnumerator IEFadeOut(Color screenColor)
        {
            fadeBoard.color = new Color(screenColor.r, screenColor.g, screenColor.b, 0f);
            fadeAni.SetTrigger("FADE_OUT");
            yield return halfSec;
        }

    }
}