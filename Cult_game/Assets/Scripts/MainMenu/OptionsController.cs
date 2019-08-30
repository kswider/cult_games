using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    private PlayerController _playerController;

    public Slider slider;
    
    private void Start()
    {
        _playerController = Utilities.FindPlayer();

        float sliderValue = (_playerController.Settings.DistanceThreshold - 50) / 25;
        slider.value = sliderValue;
    }

    public void ResetData()
    {
        _playerController.DeleteSave();
        _playerController.Settings.SetDefaults();
    }

    public void SetThreshold(float value)
    {
        int trueValue = (int) value * 25 + 50;
        _playerController.Settings.DistanceThreshold = trueValue;
        _playerController.Settings.SaveSettings();
    }
}
