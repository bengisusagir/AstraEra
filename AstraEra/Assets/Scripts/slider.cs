using UnityEngine;
using UnityEngine.UI;

public class slider : MonoBehaviour
{
    public Slider _slider;

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (maxValue > 0)
            _slider.value = currentValue / maxValue;
    }
}
