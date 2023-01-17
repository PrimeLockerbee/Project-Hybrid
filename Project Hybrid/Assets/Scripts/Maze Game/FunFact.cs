using System;
using UnityEngine;

[Serializable]
public class FunFact
{
    [Tooltip("Fun fact text. Typing PERCENTAGE will fill in the percentage of cleaned tiles")]
    public string text;

    [Range(0, 100)] public int minPercentage = 0;
    [Range(0, 100)] public int maxPercentage = 100;

    public bool isPercentageInRange(int _percentage)
    {
        return _percentage >= minPercentage && _percentage <= maxPercentage;
    }
}
