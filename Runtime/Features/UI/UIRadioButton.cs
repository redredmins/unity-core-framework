using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace RedMinS.UI
{
    [Serializable]
    public abstract class UIRadioButton : MonoBehaviour
    {
        public bool isOn { protected set; get; }

        protected UnityAction<bool> TurnAction = null;

        [SerializeField] protected Button button;
        [SerializeField] protected Image imgBtn;

        public virtual void SetButton(bool isOn, UnityAction<bool> turnAction)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClickButton());

            this.isOn = isOn;
            this.TurnAction = turnAction;

            if (isOn) TurnOn();
            else TurnOff();
        }

        public void OnClickButton()
        {
            if (isOn)
            {
                isOn = false;
                TurnOff();
            }
            else
            {
                isOn = true;
                TurnOn();
            }

            //SoundManager.Instance.PlayEffectSound(SoundManager.Instance.soundClips.BTN_Radio);
        }

        protected virtual void TurnOn()
        {
            if (TurnAction != null) TurnAction(true);
        }

        protected virtual void TurnOff()
        {
            if (TurnAction != null) TurnAction(false);
        }

    }
}