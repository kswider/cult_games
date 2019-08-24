using System;
using Resources;
using UnityEngine;

public class Settings
{
    private Place _selectedPlace;

    public Settings()
    {
        if (!PlayerPrefs.HasKey("threshold"))
        {
            SetDefaults();
            SaveSettings();
        }
        else
        {
            DistanceThreshold = PlayerPrefs.GetInt("threshold");
            LookedType = PlayerPrefs.GetString("lookedType");
            SelectedPlaceId = PlayerPrefs.GetInt("selectedPlaceId");
        }
    }
    
    public Place SelectedPlace
    {
        get => _selectedPlace;
        set
        {
            if (value != null)
            {
                SelectedPlaceId = value.id;
            }
            _selectedPlace = value;
        }
    }

    public int SelectedPlaceId { get; set; }
    
    public int DistanceThreshold { get; set; }

    public String LookedType { get; set; }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("threshold", DistanceThreshold);
        PlayerPrefs.SetString("lookedType", LookedType);
        PlayerPrefs.SetInt("selectedPlaceId", SelectedPlaceId);
        PlayerPrefs.Save();
    }

    public void SetDefaults()
    {
        LookedType = "PLACE";
        DistanceThreshold = 100;
        SelectedPlaceId = -1;
        SelectedPlace = null;
        
        SaveSettings();
    }


}
