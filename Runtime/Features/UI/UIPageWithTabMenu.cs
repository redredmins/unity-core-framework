using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RedMinS;

namespace RedMinS.UI
{
    public abstract class UIPopupWithTabMenu : UIPopup
    {
        [SerializeField] protected UITabMenu tabMenu;

        protected UnityAction[] tabActions;
        //protected GameManager manager;
        

        void Awake()
        {
            InitTabActions();
            //manager = GameManager.Instance;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            tabMenu.InitTabButton(0, ShowMenu);

            //Debug.Log(this.name + " OnEnable");
        }

        protected abstract void InitTabActions();
        protected abstract void ShowMenu(int i);

    }
}