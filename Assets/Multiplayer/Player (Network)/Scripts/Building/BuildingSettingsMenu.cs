using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BuildingSettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider movingSpeedSlider;
    [SerializeField] private TMP_Text movingSpeedValue;

    [SerializeField] private Slider rotationSpeedSlider;
    [SerializeField] private TMP_Text rotationSpeedValue;

    public void SetBuildingMoveSpeed(){
        PlayerPrefs.SetFloat("BuildingMoveSpeed", movingSpeedSlider.value);
        movingSpeedValue.text = Math.Round(movingSpeedSlider.value, 2, MidpointRounding.AwayFromZero).ToString();
    }

    public void SetBuildingRotationSpeed(){
        PlayerPrefs.SetFloat("BuildingRotationSpeed", rotationSpeedSlider.value);
        rotationSpeedValue.text = Math.Round(rotationSpeedSlider.value, 2, MidpointRounding.AwayFromZero).ToString();
    }
}