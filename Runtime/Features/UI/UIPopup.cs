using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace RedMinS
{
    public abstract class UIPopup : MonoBehaviour
    {
        //public bool dependScene = false;   // 씬에 고정으로 두면 체크

        protected static UIManager _ui;
        protected static StringTable _uiString;
        protected static SoundManager _sound;


        protected virtual void Awake()
        {
            _ui = Core.app.ui;
            _uiString = Core.app.table.uiString;
            _sound = Core.app.sound;
        }

        protected virtual void OnEnable()
        {
            _ui.OnPopup(this);
        }

        protected virtual void OnDisable()
        {
            _ui.OffPopup(this);
        }

        protected void PlayButtonSound()
        {
            _sound.PlayEffectSound("Button");
        }

        // Back키
        public virtual void OnBackKeyAction()
        {
            ClosePopup();
        }

        // 팝업창 닫기
        public virtual void ClosePopup()
        {
            gameObject.SetActive(false);
        }
    }
}