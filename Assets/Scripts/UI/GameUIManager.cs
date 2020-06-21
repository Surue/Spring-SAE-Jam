using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private Image speedCounter;
    [SerializeField] private Image lifeSlider;

    public void DisplaySpeed(float speedRatio)
    {
        speedCounter.fillAmount = speedRatio;
    }

    public void DisplayLife(float maxLife, float currentLife)
    {
        lifeSlider.fillAmount = currentLife/maxLife;
    }
}
