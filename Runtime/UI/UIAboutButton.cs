using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RedMinS
{
    public class UIAboutButton : MonoBehaviour
    {
        [SerializeField] int aboutTextId;
        [SerializeField] GameObject aboutTextBalloon;
        [SerializeField] TextMeshProUGUI txtAbout;

        StringTable _table;


        void Awake()
        {
            _table = Core.app.table.uiString;
        }

        public void ClickAbout()
        {
            aboutTextBalloon.SetActive(true);
            txtAbout.text = _table.GetString(aboutTextId);

            Invoke("HideTextBalloon", 2f);
        }

        void HideTextBalloon()
        {
            aboutTextBalloon.SetActive(false);
        }
    }
}
