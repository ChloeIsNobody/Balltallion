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
            SetSliderValue(1.0f);
        }

        private void Update()
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                SetSliderValue(slider.value - 0.1f);
            }
            
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                SetSliderValue(slider.value + 0.1f);
            }
        }

        private void SetSliderValue(float value)
        {
            slider.value = Mathf.Clamp(value, 0.1f, 1.0f);
            timeScaleText.text = $"TimeScale: {slider.value:F2}";
            Time.timeScale = slider.value;
        }
        
        private void OnSliderValueChanged(float value)
        {
            SetSliderValue(value);
        }
    }
}
