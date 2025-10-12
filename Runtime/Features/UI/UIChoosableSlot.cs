using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RedMinS
{
    public abstract class UIChoosableSlot : MonoBehaviour
    {
        private event UnityAction<UIChoosableSlot> OnChooseSlot;


        protected void OnDisable()
        {
            OnChooseSlot = null;
        }

        protected void InitSlot(UnityAction<UIChoosableSlot> chooseAction)
        {
            OnChooseSlot = chooseAction;
        }

        public void ChooseSlot()
        {
            if (OnChooseSlot != null) OnChooseSlot(this);
        }

        public void UnchoiceSlot()
        {

        }
    }
}