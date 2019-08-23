using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    private PlayerController _playerController;

    public Slider slider;
    
    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerController = player.GetComponent<PlayerController>();
        }
        float sliderValue = (_playerController.DistanceThreshold - 50) / 25;
        slider.value = sliderValue;
    }

    public void ResetData()
    {
        _playerController.DeleteSave();
    }

    public void setThreshold(float value)
    {
        int trueValue = (int) value * 25 + 50;
        _playerController.DistanceThreshold = trueValue;
    }
}
