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
        _value = Helpers.Map(0, 1, .3f, 1, _value);
        if (slider != null)
        {
            slider.value = _value;
        }
    }
}
