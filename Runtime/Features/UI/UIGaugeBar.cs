using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace RedMinS
{
    public class UIGaugeBar : MonoBehaviour
    {
        [SerializeField] Image gauge = null;

        [Header("- optional")]
        [SerializeField] TextMeshProUGUI txtGauge = null;


        public void SetGaugeBar(int count, int max)
        {
            float progress = 0f;
            if (count > 0) progress = (float)count / (float)max;
            SetGaugeText(count, max);
            SetGaugeBar(progress);
        }

        public void SetGaugeBar(float normalizedProgress)
        {
            gauge.fillAmount = normalizedProgress;
        }

        public void UpdateGaugeBar(int count, int max)
        {
            float progress = 0f;
            if (count > 0) progress = (float)count / (float)max;
            SetGaugeText(count, max);
            UpdateGaugeBar(progress);
        }

        public void UpdateGaugeBar(float normalizedProgress)
        {
            gauge.DOFillAmount(normalizedProgress, 0.5f);
        }

        public void UpdateGaugeBarInTime(float normalizedProgress, TimeSpan time)
        {
            UpdateGaugeBarInTime(normalizedProgress, (float)time.TotalSeconds);
        }

        public void UpdateGaugeBarInTime(float normalizedProgress, float time)
        {
            gauge.DOFillAmount(normalizedProgress, time);
        }

        void SetGaugeText(int count, int max)
        {
            if (txtGauge == null) return;
            
            if (max > 0)
                txtGauge.text = $"({count}/{max})";
            else
                txtGauge.text = "MAX";
        }
    }
}