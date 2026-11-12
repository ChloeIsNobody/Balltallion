using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Balltallion.DebugTools
{
    public class DebugTimeScaleSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI timeScaleText;

        private void OnEnable()
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        private void Awake()
        {
            SetSliderValue(0.5f);
        }

        private void SetSliderValue(float value)
        {
            slider.value = Mathf.Clamp(value, 0.0f, 1.0f);
            if (slider.value > 0.45f && slider.value < 0.55f) slider.value = 0.5f;
            
            float timeValue = slider.value * 2.0f;
            timeScaleText.text = $"TimeScale: {timeValue:F2}";
            Time.timeScale = timeValue;
        }
        
        private void OnSliderValueChanged(float value)
        {
            SetSliderValue(value);
        }
    }
}
