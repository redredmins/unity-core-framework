using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RedMinS
{
    public abstract class UIChoosableListItem : MonoBehaviour
    {
        private event UnityAction<UIChoosableListItem> OnChooseListItem;


        protected void OnDisable()
        {
            OnChooseListItem = null;
        }

        protected void InitListItem(UnityAction<UIChoosableListItem> chooseAction)
        {
            OnChooseListItem = chooseAction;
        }

        public void ChooseListItem()
        {
            if (OnChooseListItem != null) OnChooseListItem(this);
        }

        public void UnchoiceListItem()
        {

        }
    }
}