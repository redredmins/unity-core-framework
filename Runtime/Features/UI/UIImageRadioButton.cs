using UnityEngine;
using UnityEngine.Events;

namespace RedMinS.UI
{
    public class UIImageRadioButton : UIRadioButton
    {
        [SerializeField] Sprite onStateSprite;
        [SerializeField] Sprite offStateSprite;


        public override void SetButton(bool isOn, UnityAction<bool> turnAction)
        {
            base.SetButton(isOn, turnAction);
            //Debug.Log("Image radio button set");
        }

        protected override void TurnOn()
        {
            imgBtn.sprite = onStateSprite;
            //Debug.Log("Image radio button turn on");

            base.TurnOn();
        }

        protected override void TurnOff()
        {
            imgBtn.sprite = offStateSprite;
            //Debug.Log("Image radio button turn off");

            base.TurnOff();
        }
    }
}