using UnityEngine;
using UnityEngine.UI;

namespace RedMinS.UI
{
    public class UIImageText : MonoBehaviour
    {
        [SerializeField] Image imgNumberOne = null;
        [SerializeField] Image imgNumberTen = null;
        [SerializeField] Vector3[] numberPos = new Vector3[2];
        [SerializeField] Sprite[] numberSprites = null;


        public void SetText(int number)
        {
            if (number >= 0 && number < 10)
            {
                imgNumberOne.gameObject.SetActive(true);
                imgNumberOne.rectTransform.localPosition = new Vector3(0f, 0f, 0f);
                imgNumberOne.sprite = numberSprites[number];

                imgNumberTen.gameObject.SetActive(false);
            }
            else if (number > 9)
            {
                int tenNumber = number / 10;
                int oneNumber = number % 10;

                imgNumberOne.gameObject.SetActive(true);
                imgNumberOne.rectTransform.localPosition = numberPos[0];
                imgNumberOne.sprite = numberSprites[oneNumber];

                imgNumberTen.gameObject.SetActive(true);
                imgNumberTen.rectTransform.localPosition = numberPos[1];
                imgNumberTen.sprite = numberSprites[tenNumber];
            }
            else
            {
                imgNumberOne.gameObject.SetActive(true);
                imgNumberOne.rectTransform.localPosition = numberPos[0];
                imgNumberOne.sprite = numberSprites[0];

                imgNumberTen.gameObject.SetActive(true);
                imgNumberTen.rectTransform.localPosition = numberPos[1];
                imgNumberTen.sprite = numberSprites[0];
            }
        }
    }
}