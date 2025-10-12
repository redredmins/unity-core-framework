using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RedMinS.UI
{
    public class UITabMenu : MonoBehaviour
    {
        public enum TabType
        {
            DifferentSprite,
            DifferentColor,
            DifferentTextColor,
            DifferentSize,
        }

        [SerializeField] TabType[] tabTypes;

        [Header("- tab buttons")]
        [SerializeField] Button[] tabButtons;
        [SerializeField] Image[] imgTabBgs;
        [SerializeField] TextMeshProUGUI[] txtBtnLables;

        [Header("- on/off")]
        [Tooltip("0: off / 1: on")]
        [SerializeField] Sprite[] btnBgSprite;  // 0: off | 1: on
        [SerializeField] Color[] btnBgColor;    // 0: off | 1: on
        [SerializeField] Color[] btnTextColor;  // 0: off | 1: on
        [SerializeField] Vector2[] btnBgSize;   // 0: off | 1: on

        [Header("- tab contents")]
        [SerializeField] bool isSelfShow = false;
        [SerializeField] int firstShownIndex = 0;
        [SerializeField] UITabMenuContents[] tabContents;

        [Header("- optional")]
        [SerializeField] ScrollRect tabContentsList;

        Action<int> OnTabButtonAction;
        public delegate bool CanTabHandler(int index);
        CanTabHandler OnCanTab;

        Button _curTab;
        SoundManager _sound;
        string _clickSoundLabel;
        

        void OnEnable()
        {
            if (isSelfShow == true)
            {
                InitTabButton(firstShownIndex, SetTabContents);
            }
        }

        void SetTabContents(int index)
        {
            for (int i = 0; i < tabContents.Length; ++i)
            {
                tabContents[i].gameObject.SetActive(i == index);
                if(i == index)
                {
                    tabContents[i].ShowContents();

                    if (tabContentsList != null)
                    {
                        float height = tabContents[i].GetComponent<RectTransform>().sizeDelta.y;
                        tabContentsList.content.sizeDelta = new Vector2(tabContentsList.content.sizeDelta.x, height);
                    }
                }
            }
            
            if (tabContentsList != null)
                tabContentsList.verticalNormalizedPosition = 1f;
        }

        public void InitTabButton(int onButtonIndex, Action<int> onTabAction, CanTabHandler onCanTab = null) // enum
        {
            //Debug.Log("InitTabButton");
            if (_sound == null) _sound = Core.app.sound;

            _clickSoundLabel = "Button";
            _curTab = tabButtons[onButtonIndex];
            OnTabButtonAction = onTabAction;
            OnCanTab = onCanTab;

            for (int i = 0; i < tabButtons.Length; ++i)
            {
                SetButtonLook(i, (i == onButtonIndex));
            }

            OnTabButtonAction(onButtonIndex);
        }

        // 탭버튼 눌렀을 때
        public void ClickTabButton(Button button)
        {
            int index = 0;
            for (int t = 0; t < tabButtons.Length; ++t)
                if (tabButtons[t] == button) index = t;

            if ((button != _curTab)
                && (OnCanTab == null || OnCanTab(index) == true))
            {
                _sound.PlayEffectSound(_clickSoundLabel);

                for (int i = 0; i < tabButtons.Length; ++i)
                {
                    if (i == index) //tabButtons[i] == button)
                    {
                        _curTab = tabButtons[i];
                        SetButtonLook(i, true);
                        OnTabButtonAction(i);
                        //AnalyticsManager.DebugLog(i + "번 버튼 눌려짐");
                    }
                    else
                    {
                        SetButtonLook(i, false);
                        //AnalyticsManager.DebugLog(i + "번 버튼 비활성");
                    }
                }
            }
        }

        void SetButtonLook(int btnIndex, bool isOn)
        {
            foreach (TabType t in tabTypes)
            {
                int i = (isOn) ? 1 : 0;

                switch (t)
                {
                    case TabType.DifferentSprite:
                        imgTabBgs[btnIndex].sprite = btnBgSprite[i];
                        break;

                    case TabType.DifferentColor:
                        imgTabBgs[btnIndex].color = btnBgColor[i];
                        break;

                    case TabType.DifferentTextColor:
                        txtBtnLables[btnIndex].color = btnTextColor[i];
                        break;

                    case TabType.DifferentSize:

                        imgTabBgs[btnIndex].rectTransform.sizeDelta = btnBgSize[i];
                        break;
                }

            }
        }

    }

}