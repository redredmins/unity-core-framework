using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RedMinS
{
    //[AddComponentMenu("Custom/UI/TextLabel")]
    //[RequireComponent(typeof(TextMeshProUGUI))]
    public class UITextLabel : MonoBehaviour
    {
        //[Header("Lable Info")]
        //public TableType tableType { set; get; }

        [Tooltip("uiString 테이블 아이디")]
        [SerializeField] int tableUid; //{ set; get; }
        TextMeshProUGUI lable;

        static StringTable _table;
        static bool isTableInit = false;


        IEnumerator Start()
        {
            if (Core.app.table.isLoaded == false)
            {
                yield return new WaitUntil(() => { return Core.app.table.isLoaded; });
                isTableInit = true;
            }

            _table = Core.app.table.uiString;
            lable = GetComponent<TextMeshProUGUI>();
            SetText();
        }

        private void OnEnable()
        {
            if (isTableInit)
            {
                SetText();
            }
        }

        public void UpdateText()
        {
            SetText();
        }

        void SetText()
        {
            if (lable != null && tableUid != 0)
            {
                lable.text = _table.GetString(tableUid);
            }
        }
    }
}