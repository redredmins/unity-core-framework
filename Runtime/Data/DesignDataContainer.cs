using UnityEngine;
using System;
using System.Collections.Generic;

namespace RedMinS
{
    public enum Language : int
    {
        None = 0,
        Korean = 1,
        English,
        Japanese,
        MAX
    }

    [Serializable]
    public class LocalizationTable
    {
        public Language language;
        public TextAsset uiString;
    }

    public class DesignDataContainer : MonoBehaviour
    {
        // 프레임워크 공통 스트링 테이블 경로 (Runtime/Resources 아래)
        const string COMMON_TABLE_PATH = "RedCoreFramework/{0}_common_stringtable";

        [Header("- items")]
        [SerializeField] TextAsset itemTableText;


        // 국가별 언어 (게임별 테이블. 공통 문구는 프레임워크 공통 테이블이 우선)
        [Header("Localization")]
        [SerializeField] LocalizationTable krTable;
        [SerializeField] LocalizationTable enTable;
        [SerializeField] LocalizationTable jpTable;

        public Language curLang { private set; get; }

        // 테이블
        // public ObjectTable<ProductInfo> product { private set; get; }
        // public ObjectTable<ItemInfo> item { private set; get; }

        StringTable _uiString;
        public StringTable uiString
        {
            get
            {
                if (!isLoaded) LoadAllTable(); // Awake 순서와 무관하게 안전
                return _uiString;
            }
        }

        //[ShowOnly]
        public bool isLoaded = false;


        void Awake()
        {
            LoadAllTable();
        }

        // 모든 테이블 불러오기
        public void LoadAllTable()
        {
            if (isLoaded) return;

            curLang = (Language)PlayerPrefs.GetInt("LANGUAGE", 0);
            if (curLang == Language.None)
            {
                InitLanguage();
            }

            //userLevel = new LevelTable(userLevelTableText);
            BuildUiStringTable();

            isLoaded = true;
        }

        // 프레임워크 공통 테이블 우선 + 게임 테이블에서 없는 키만 보충
        void BuildUiStringTable()
        {
            TextAsset common = Resources.Load<TextAsset>(
                string.Format(COMMON_TABLE_PATH, LanguageLabel(curLang)));

            var gameTable = GetLocalizationTable(curLang);
            TextAsset gameText = (gameTable != null) ? gameTable.uiString : null;

            if (common != null)
            {
                _uiString = new StringTable(common);
                if (gameText != null)
                {
                    _uiString.MakeStringTable(gameText); // 기존 키는 유지 (first-wins)
                }
            }
            else if (gameText != null)
            {
                Debug.LogWarning($"[DesignDataContainer] {curLang} 공통 테이블 없음 - 게임 테이블만 사용");
                _uiString = new StringTable(gameText);
            }
            else
            {
                Debug.LogError($"[DesignDataContainer] {curLang} 스트링 테이블 없음");
                _uiString = new StringTable(new TextAsset[0]);
            }
        }

        // 언어 바꾸기
        public void ChangeLanguage(Language lang)
        {
            if (curLang != lang)
            {
                curLang = lang;
                PlayerPrefs.SetInt("LANGUAGE", (int)curLang);

                BuildUiStringTable();
            }
        }

        // 언어
        void InitLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Korean:
                    curLang = Language.Korean;
                    break;
                case SystemLanguage.Japanese:
                    curLang = Language.Japanese;
                    break;

                default:
                    curLang = Language.English;
                    break;
            }
        }

        LocalizationTable GetLocalizationTable(Language lang)
        {
            switch (lang)
            {
                case Language.Korean: return krTable;
                case Language.English: return enTable;
                case Language.Japanese: return jpTable;

                default: return krTable;
            }
        }

        public static string LanguageLabel(Language lang)
        {
            switch (lang)
            {
                case Language.Korean: return "kr";
                case Language.English: return "en";
                case Language.Japanese: return "jp";

                default: return "en";
            }
        }
    }
}
