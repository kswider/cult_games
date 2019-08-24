using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Resources;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    //Save
    public int Score { get; set; }
    public List<int> DiscoveredPlaces { get; set; }
    public List<Save.PlaceBlock> BlockedPlaces { get; set; }

    //Database
    public List<Place> places;
    
    //PlayerSettings
    public Settings Settings { get; private set; }

    private void Start()
    {
        Settings = new Settings();
        Place selected = places.Find(p => p.id == Settings.SelectedPlaceId);
        if (selected != null)
        {
            Settings.SelectedPlace = selected;
        }
        LoadGame();
    }



    public void SaveGame()
    {
        Save save = CreateSaveGameObject();
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();
        
        Debug.Log("Game Saved");
    }
    
    public void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            File.Delete(Application.persistentDataPath + "/gamesave.save");
        }
        Score = 0;
        DiscoveredPlaces = new List<int>();
        BlockedPlaces = new List<Save.PlaceBlock>();
        Debug.Log("Game Data Deleted");
    }

    public void AddPoints(int points)
    {
        Score += points;
    }
    
    private Save CreateSaveGameObject()
    {
        Save save = new Save
        {
            generalScore = Score
            , discoveredPlaces = DiscoveredPlaces
            , blockedPlaces = BlockedPlaces
        };
        return save;
    }

    private void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            Score = save.generalScore;
            DiscoveredPlaces = save.discoveredPlaces;
            BlockedPlaces = save.blockedPlaces;
            
            Debug.Log("Game Loaded");
        }
        else
        {
            Score = 0;
            DiscoveredPlaces = new List<int>();
            BlockedPlaces = new List<Save.PlaceBlock>();
            Debug.Log("No game saved!");
        }
    }
}
