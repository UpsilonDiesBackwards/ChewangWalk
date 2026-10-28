using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsBinder : MonoBehaviour, IEndDragHandler {
    [Tooltip("Reference to volume slider")] 
    [SerializeField] private Slider slider;

    [Tooltip("Text element used to display current value of slider")] 
    [SerializeField] private TextMeshProUGUI valueText;
    
    [Header("Formatting")]
    [Tooltip("If true, the value label will format the float as a percentage (0 to 1 becomes 0% to 100%)")]
    [SerializeField] private bool displayAsPercentage = true;

    [Tooltip("If true, the value label will prefix the value with an 'x' multiplier symbol")]
    [SerializeField] private bool displayAsMultiple = false;

    [Tooltip("Minimum value of the slider")]
    [SerializeField] private float minValue = 0.0f;
    
    [Tooltip("Maximum value of the slider")]
    [SerializeField] private float maxValue = 1.0f;
    
    [Tooltip("How many decimal places the label can show")]
    [Range(0, 3)]
    [SerializeField] private int decimalPlaces = 2;
    
    private Action _onDragEnded;

    public void Initialise(Action<float> onValueChanged, Action onDragEnded = null) {
        if (slider == null) return;

        _onDragEnded = onDragEnded;
        
        float range = maxValue - minValue;
        slider.wholeNumbers = decimalPlaces == 0 && range > 1f;
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        
        slider.onValueChanged.AddListener(val => {
            float step = _getSliderStepSize();
            float snappedVal = Mathf.Round(val / step) * step;
            
            slider.SetValueWithoutNotify(snappedVal);
            _updateLabel(snappedVal);
            onValueChanged?.Invoke(snappedVal);
        });
        
        EventTrigger trigger = slider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) {
            trigger = slider.gameObject.AddComponent<EventTrigger>();
        }
        
        trigger.triggers.RemoveAll(entry => entry.eventID == EventTriggerType.PointerUp);

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => {
            _onDragEnded?.Invoke();
        });
        
        trigger.triggers.Add(entry);
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        _onDragEnded?.Invoke();
    }

    public void SetValueSilently(float value) {
        if (slider != null) {
            slider.SetValueWithoutNotify(value);
        }
        _updateLabel(value);
    }

    private void _updateLabel(float value) {
        if (valueText == null) return;

        if (displayAsPercentage) {
            valueText.text = Mathf.RoundToInt(value * 100) + "%";
        }
        else {
            string formattedValue = value.ToString("G" + (decimalPlaces > 0 ? decimalPlaces + 1 : ""));
            valueText.text = displayAsMultiple ? "x" + formattedValue : formattedValue;
        }
    }
    
    private float _getSliderStepSize() {
        return 1f / Mathf.Pow(10, decimalPlaces);
    }
}