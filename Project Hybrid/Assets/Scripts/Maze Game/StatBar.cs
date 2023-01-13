using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetSliderValue(float _value)
    {
        _value = Mathf.Clamp01(_value);
        if (slider != null)
        {
            slider.value = _value;
        }
    }
}
