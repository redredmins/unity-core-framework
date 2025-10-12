using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;

namespace RedMinS.UI
{
    //[RequireComponent (typeof (Button))]
    public class UITextRadioButton : UIRadioButton
    {
        public Color onStateBg;
        public Color onStateText;
        public Color offStateBg;
        public Color offStateText;

        [SerializeField] TextMeshProUGUI txtBtnOnOff = null;


        public override void SetButton(bool isOn, UnityAction<bool> turnAction)
        {
            //txtBtnOnOff = transform.GetComponentInChildren<Text>();

            base.SetButton(isOn, turnAction);
        }

        protected override void TurnOn()
        {
            imgBtn.color = onStateBg;
            txtBtnOnOff.color = onStateText;
            //text.text = Container.Instance.table.uiStringTable.GetString();

            base.TurnOn();
        }

        protected override void TurnOff()
        {
            imgBtn.color = offStateBg;
            txtBtnOnOff.color = offStateText;
            //text.text = Container.Instance.table.uiStringTable.GetString();

            base.TurnOff();
        }

    }
}