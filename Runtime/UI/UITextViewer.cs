using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RedMinS
{
    [Serializable]
    public class TextViewData
    {
        [SerializeField] string textKey;
        public string Key
        {
            get { return textKey; }
        }

        [SerializeField] TextMeshProUGUI textRenderer;
        public TextMeshProUGUI Renderer
        {
            get { return textRenderer; }
        }
    }

    public class UITextViewer : MonoBehaviour
    {
        [SerializeField] TextViewData[] textViewers;


        public void SetText(string key, string text)
        {
            foreach (var textViewer in textViewers)
            {
                if (key == textViewer.Key)
                {
                    textViewer.Renderer.text = text;
                }
            }
        }
    }
}
