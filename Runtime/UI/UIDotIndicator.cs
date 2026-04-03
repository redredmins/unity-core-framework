using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace RedMinS
{
    public class UIDotIndicator : MonoBehaviour
    {
        [SerializeField] Image[] imgDots;
        [SerializeField] Color activeDotColor;
        [SerializeField] Color inactiveDotColor;

        
        public void SetIndicator(int index)
        {
            for (int i = 0; i < imgDots.Length; ++i)
            {
                if (i == index) imgDots[i].color = activeDotColor;
                else imgDots[i].color = inactiveDotColor;
            }
        }
    }
}
