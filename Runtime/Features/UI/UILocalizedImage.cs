using UnityEngine;
using UnityEngine.UI;

namespace RedMinS.UI
{
    public class UILocalizedImage : MonoBehaviour
    {
        [SerializeField] Image image;

        [SerializeField] Sprite kr = null;
        [SerializeField] Sprite en = null;
        //[SerializeField] Sprite jp = null;
        //[SerializeField] Sprite tw = null;


        void OnEnable()
        {
            SetImage(Core.app.table.curLang);
        }

        void SetImage(Language lang)
        {
            Sprite s = null;
            switch (lang)
            {
                //case Language.None:
                //    s = en;
                //    break;
                case Language.Korean:
                    s = kr;
                    break;
                //case Language.English:
                //    s = en;
                //    break;

                    /*
                    case Language.Japanese:
                        s = jp;
                        break;
                    case Language.ChineseTW:
                        s = tw;
                        break;
                    */
                
                default:
                    s = kr;
                    break;
            }

            image.sprite = s;
            image.SetNativeSize();
            //image.GetComponent<RectTransform>().sizeDelta = new Vector2(s.bounds.size.x * 100, s.bounds.size.y * 100);
        }
    }
}