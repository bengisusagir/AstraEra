using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class slider : MonoBehaviour
{
    public Slider _slider;




    public void UpdateHealthBar(float currentValue , float maxValue)
    {
        _slider.value = currentValue / maxValue;
    }
    void Update()
    {

    }
}
