using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace RedMinS
{
    public class UITextEffect
    {
        public static IEnumerator ShowTimerText(Text renderer, TimeSpan t, UnityAction doneAction)
        {
            WaitForSeconds oneSec = new WaitForSeconds(1f);
            while (renderer.gameObject.activeInHierarchy)
            {
                renderer.text = string.Format("{0}:{1:00}", t.Minutes, t.Seconds);
                yield return oneSec;

                t = t.Subtract(new TimeSpan(0, 0, 1));
                //Debug.Log("timer : " + t.TotalSeconds);

                if (t.TotalSeconds <= 0 && doneAction != null)
                {
                    doneAction();
                    break;
                }
            }
        }

        public static IEnumerator ShowTypingText(Text renderer, string text) //float during)
        {
            //SoundManager.Instance.PlayEffectSound_Typing(true);
            char[] chars = text.ToCharArray();
            string typing = "";
            renderer.text = typing;

            float t = 0.05f; //during / chars.Length;
            WaitForSeconds sec = new WaitForSeconds(t);
            for (int i = 0; i < chars.Length; ++i)
            {
                typing += chars[i];
                renderer.text = typing + "_";
                yield return sec;

                //if (Input.GetMouseButton(0)) break;
            }
            renderer.text = text;
            //SoundManager.Instance.PlayEffectSound_Typing(false);
        }
    }
}