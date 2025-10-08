using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

namespace RedMinS
{
    [System.Serializable]
    public class SystemPopupMessage
    {
        public string message = string.Empty;
        public string yes = string.Empty;
        public string no = string.Empty;
        public UnityAction yesBtnAction = null;
        public UnityAction noBtnAction = null;
    }

    public class UISystemPopup : UIPopup
    {
        [SerializeField] TextMeshProUGUI txtContent = null;

        [Header("- 1 button set")]
        [SerializeField] GameObject oneBtnSet = null;
        [SerializeField] Button btnOne = null;
        [SerializeField] TextMeshProUGUI labBtnOne = null;

        [Header("- 2 button set")]
        [SerializeField] GameObject twoBtnSet = null;
        [SerializeField] Button btnTwoYes = null;
        [SerializeField] Button btnTwoNo = null;
        [SerializeField] TextMeshProUGUI labBtnTwoYes = null;
        [SerializeField] TextMeshProUGUI labBtnTwoNo = null;

        UnityAction _backKeyAction = null;


        protected override void OnEnable()
        {
            base.OnEnable();

            btnOne.onClick.AddListener(PlayButtonSound);
            btnTwoYes.onClick.AddListener(PlayButtonSound);
            btnTwoNo.onClick.AddListener(PlayButtonSound);
        }

        protected override void OnDisable()
        {
            btnOne.onClick.RemoveAllListeners();
            btnTwoYes.onClick.RemoveAllListeners();
            btnTwoNo.onClick.RemoveAllListeners();

            oneBtnSet.SetActive(false);
            twoBtnSet.SetActive(false);

            base.OnDisable();
        }

        public override void OnBackKeyAction()
        {
            //base.OnBackKeyAction();

            if (_backKeyAction != null)
            {
                _backKeyAction();
            }
            else
                base.OnBackKeyAction();
        }


        public void SetSystemPopup(SystemPopupMessage systemMessage, UnityAction<string, GameObject> removeSelf)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = new Vector3(1f, 1f, 1f);

            txtContent.text = systemMessage.message;
            UnityAction removeSelfAction = () => { removeSelf(systemMessage.message, this.gameObject); };

            if (systemMessage.yes != string.Empty)
            {
                if (systemMessage.no != string.Empty) // --- yes, no 버튼 다 있으면
                {
                    oneBtnSet.SetActive(false);
                    twoBtnSet.SetActive(true);
                    labBtnTwoYes.text = systemMessage.yes;
                    labBtnTwoNo.text = systemMessage.no;
                    if (systemMessage.yesBtnAction != null)
                    {
                        btnTwoYes.onClick.AddListener(systemMessage.yesBtnAction);
                    }
                    btnTwoYes.onClick.AddListener(removeSelfAction);
                    if (systemMessage.noBtnAction != null)
                    {
                        btnTwoNo.onClick.AddListener(systemMessage.noBtnAction);
                        _backKeyAction = systemMessage.noBtnAction;
                    }
                    btnTwoNo.onClick.AddListener(removeSelfAction);
                    _backKeyAction += removeSelfAction;
                }
                else        // --- yes 버튼 있으면
                {
                    oneBtnSet.SetActive(true);
                    twoBtnSet.SetActive(false);
                    labBtnOne.text = systemMessage.yes;
                    if (systemMessage.yesBtnAction != null)
                    {
                        btnOne.onClick.AddListener(systemMessage.yesBtnAction);
                        _backKeyAction = systemMessage.yesBtnAction;
                    }
                    btnOne.onClick.AddListener(removeSelfAction);
                    _backKeyAction += removeSelfAction;
                }
            }
            else            // --- 메시지 확인만 가능
            {
                oneBtnSet.SetActive(true);
                twoBtnSet.SetActive(false);
                labBtnOne.text = Core.app.table.uiString.GetString(1001);
                btnOne.onClick.AddListener(removeSelfAction);
                _backKeyAction = removeSelfAction;
            }
        }
    }
}